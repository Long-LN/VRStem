using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class SolarSystemFocus : MonoBehaviour
{
    public static SolarSystemFocus Instance;
    public Transform solarRoot;

    [FormerlySerializedAs("quizManager")] public QuizAndInforManager quizAndInforManager;
    public PlanetController planetController;
    public Handle handle;

    public float zoomSpeed = 1f;
    public float minScale = 0.5f;
    public float targetScale = 100f;
    public float modelAppearScale = 1f;
    public float questionAppearScale = 95f;

    private bool showModel = false;
    private bool showInfor = false;

    public Transform pivot;
    public bool focusIn;
    public bool focusOut;

    public PlanetVisual planetVisual;
    public PlanetSelectable planetSelectable;

    [Header("─── Zoom To Camera ───")]
    public float distanceFromCamera = 1.5f;

    // Trạng thái gốc của solarRoot
    private Vector3 _originPos;
    private Quaternion _originRot;
    private Vector3 _originScale;

    // Big visual đang được focus
    private PlanetVisual _currentBigVisual;

    // Lưu trạng thái gốc của model
    private Transform _modelParent;
    private Vector3 _modelLocalPos;
    private Quaternion _modelLocalRot;
    private Vector3 _modelLocalScale;

    // Vị trí world của model lúc bắt đầu focus — cache 1 lần duy nhất
    private Vector3 _modelOriginWorld;

    // Vị trí đích trước camera
    private Vector3 _focusWorldPos;

    // t hiện tại [0..1]
    private float _currentT = 0f;

    private void Awake()
    {
        Instance = this;
        pivot = solarRoot;
    }

    private void Start()
    {
        _originPos = solarRoot.position;
        _originRot = solarRoot.rotation;
        _originScale = solarRoot.localScale;

        solarRoot.localScale = Vector3.one * minScale;
        _currentT = 0f;
        handle.UpdateHandleByScale(minScale);
    }

    // ── Cho PlanetQuiz lấy vị trí hành tinh đang focus để đặt canvas ──
    public Vector3 GetFocusWorldPos() => _focusWorldPos;

    public void SetSystemScale(float progress)
    {
        _currentT = Mathf.Clamp01(progress / 100f);
        float newScale = Mathf.Lerp(minScale, targetScale, _currentT);
        solarRoot.localScale = Vector3.one * newScale;
        pivot = solarRoot;
    }

    // ── LateUpdate: CHỈ chạy khi zoom IN ──
    // Zoom out không cần — model đã về local, thu nhỏ tự nhiên theo solarRoot
    private void LateUpdate()
    {
        if (!focusIn) return;
        if (_currentBigVisual == null || _currentBigVisual.model == null) return;

        // Lerp model từ vị trí gốc → trước camera
        _currentBigVisual.model.transform.position = Vector3.Lerp(
            _modelOriginWorld,
            _focusWorldPos,
            _currentT
        );
    }

    // ── FocusPlanet ──
    public void FocusPlanet(Transform planet, PlanetVisual visual)
    {
        StopAllCoroutines();
        focusIn = false;
        focusOut = false;

        // 1. Trả model cũ về local + tắt orbit cũ TRƯỚC khi reset solarRoot
        RestoreCurrentBigVisual();

        // 2. Reset solarRoot (phải sau Restore để world pos tính đúng)
        solarRoot.position = _originPos;
        solarRoot.rotation = _originRot;
        solarRoot.localScale = Vector3.one * minScale;
        _currentT = 0f;
        pivot = solarRoot;

        // 3. Gán mới
        planetVisual = visual;
        planetSelectable = planet.GetComponent<PlanetSelectable>();
        planetController.SetPlanetZoom(visual);

        PlanetVisual bigVisual = planetController.bigPlanets.Find(p => p.planetName == visual.planetName);
        _currentBigVisual = bigVisual;

        if (bigVisual == null || bigVisual.model == null)
        {
            Debug.LogError("[FocusPlanet] Không tìm thấy bigVisual hoặc model cho: " + visual.planetName);
            return;
        }

        // 4. Tắt orbit big planet để không ghi đè position trong lúc zoom
        PlanetOrbit[] orbits = bigVisual.GetComponentsInParent<PlanetOrbit>();
        foreach (var o in orbits) o.enabled = false;

        // 5. Lưu local transform gốc của model
        Transform model = bigVisual.model.transform;
        _modelParent = model.parent;
        _modelLocalPos = model.localPosition;
        _modelLocalRot = model.localRotation;
        _modelLocalScale = model.localScale;

        // 6. Cache world position của model ngay lúc này
        // (solarRoot đang ở minScale + originPos → world pos chính xác)
        _modelOriginWorld = model.position;

        // 7. Tính vị trí đích trước camera
        Camera cam = Camera.main ?? FindObjectOfType<Camera>();
        _focusWorldPos = cam != null
            ? cam.transform.position + cam.transform.forward * distanceFromCamera
            : model.position;

        if (PlanetRotator.Instance != null)
            PlanetRotator.Instance.SetPlanet(bigVisual.model);

        showModel = false;
        showInfor = false;
        focusIn = true;

        StartCoroutine(ZoomInRoutine());
    }

    // ── ZoomOut ──
    public void ZoomOut()
    {
        StopAllCoroutines();
        focusOut = true;
        focusIn = false;

        // Trả model về đúng local transform TRƯỚC khi zoom out
        // để model thu nhỏ tự nhiên cùng solarRoot, không bị orbit ghi đè
        if (_currentBigVisual != null && _currentBigVisual.model != null && _modelParent != null)
        {
            Transform m = _currentBigVisual.model.transform;
            m.SetParent(_modelParent, false);
            m.localPosition = _modelLocalPos;
            m.localRotation = _modelLocalRot;
            m.localScale = _modelLocalScale;
        }

        // KHÔNG bật lại orbit ở đây — bật SAU KHI zoom out xong
        // (nếu bật ngay, PlanetOrbit.Update() sẽ ghi đè position mỗi frame)

        StartCoroutine(ZoomOutRoutine());
    }

    // ── Coroutines ──
    private IEnumerator ZoomInRoutine()
    {
        float startT = _currentT;
        float duration = zoomSpeed * (1f - startT);

        if (duration <= 0f) { FinishZoomIn(); yield break; }

        float elapsed = 0f;
        while (elapsed < duration && focusIn)
        {
            elapsed += Time.deltaTime;
            float raw = elapsed / duration;
            float smooth = raw * raw * (3f - 2f * raw);
            SetSystemScale(Mathf.Lerp(startT, 1f, smooth) * 100f);
            handle.UpdateHandleByScale(solarRoot.localScale.x);
            yield return null;
        }

        if (focusIn) FinishZoomIn();
    }

    private void FinishZoomIn()
    {
        SetSystemScale(100f);
        handle.UpdateHandleByScale(targetScale);
    }

    private IEnumerator ZoomOutRoutine()
    {
        float startT = _currentT;
        float duration = zoomSpeed * startT;

        if (duration <= 0f) { FinishZoomOut(); yield break; }

        float elapsed = 0f;
        while (elapsed < duration && focusOut)
        {
            elapsed += Time.deltaTime;
            float raw = elapsed / duration;
            float smooth = raw * raw * (3f - 2f * raw);
            SetSystemScale(Mathf.Lerp(startT, 0f, smooth) * 100f);
            handle.UpdateHandleByScale(solarRoot.localScale.x);
            yield return null;
        }

        if (focusOut) FinishZoomOut();
    }

    private void FinishZoomOut()
    {
        SetSystemScale(0f);
        solarRoot.position = _originPos;
        solarRoot.rotation = _originRot;
        solarRoot.localScale = Vector3.one * minScale;
        handle.UpdateHandleByScale(minScale);

        // Bật lại orbit SAU KHI zoom out xong hoàn toàn
        if (_currentBigVisual != null)
        {
            PlanetOrbit[] orbits = _currentBigVisual.GetComponentsInParent<PlanetOrbit>();
            foreach (var o in orbits) o.enabled = true;
        }

        showModel = false;
        showInfor = false;
        focusIn = false;
        focusOut = false;
        _currentBigVisual = null;
        _modelParent = null;
        pivot = solarRoot;

        if (PlanetRotator.Instance != null)
            PlanetRotator.Instance.ClearPlanet();
    }

    // ── Trả model về local transform + bật lại orbit ──
    // Chỉ dùng khi chuyển focus sang hành tinh khác
    private void RestoreCurrentBigVisual()
    {
        if (_currentBigVisual == null) return;

        PlanetOrbit[] orbits = _currentBigVisual.GetComponentsInParent<PlanetOrbit>();
        foreach (var o in orbits) o.enabled = true;

        if (_currentBigVisual.model != null && _modelParent != null)
        {
            Transform m = _currentBigVisual.model.transform;
            m.SetParent(_modelParent, false);
            m.localPosition = _modelLocalPos;
            m.localRotation = _modelLocalRot;
            m.localScale = _modelLocalScale;
        }

        _currentBigVisual = null;
        _modelParent = null;
    }

    // ── Update: show/hide model & quiz ──
    void Update()
    {
        if (planetVisual == null) return;

        float s = solarRoot.lossyScale.x;

        if (s >= modelAppearScale && !showModel)
        {
            planetVisual.ShowModel();
            showModel = true;
        }
        else if (s < modelAppearScale && showModel)
        {
            planetVisual.ShowMarker();
            showModel = false;
            if (planetVisual.planetName != "Sun")
                planetSelectable.ResetFocus();
        }

        if (s >= questionAppearScale && !showInfor)
        {
            if (planetVisual.planetName == "Sun") return;
            Debug.Log($"[SolarSystemFocus] Gọi ShowPanel cho: {planetVisual.planetName}");
            quizAndInforManager.ShowPanel(planetVisual.planetName);
            showInfor = true;
        }
        else if (s < questionAppearScale && showInfor)
        {
            if (planetVisual.planetName == "Sun") return;
            quizAndInforManager.HidePanel(planetVisual.planetName);
            showInfor = false;
        }
    }

    public Transform ChangePivot(Transform objectToMove, Vector3 newPivotPosition)
    {
        return solarRoot;
    }
}