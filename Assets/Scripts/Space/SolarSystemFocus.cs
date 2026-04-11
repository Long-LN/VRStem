using System;
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
    public float minScale;
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
    
    private Vector3 _startPivotPos = Vector3.zero;
    private Vector3 _targetPivotPos = Vector3.zero;

    private void Awake()
    {
        Instance = this;
        pivot = solarRoot.parent;
    }
    

    public void SetSystemScale(float progress)
    {
        Debug.Log(progress);
        float t = progress / 100f;
        float newScale = Mathf.Lerp(minScale, targetScale, t);

        if (pivot != null)
        {
            pivot.localScale = Vector3.one * newScale;

            // CHỈ LERP VỊ TRÍ NẾU ĐANG DÙNG PIVOT ẢO (đang focus)
            // if (pivot != solarRoot) 
            // {
            //     pivot.position = Vector3.Lerp(_startPivotPos, _targetPivotPos, t);
            // }
            if(_startPivotPos != Vector3.zero)
                pivot.position = Vector3.Lerp(_startPivotPos, _targetPivotPos, t);
        }
    }

    public void FocusPlanet(Transform planet, PlanetVisual visual)
    {
        planetVisual = visual;
        planetSelectable = planet.GetComponent<PlanetSelectable>();
        planetController.SetPlanetZoom(visual);
        // dùng hàm ChangePivot
        Debug.Log(planet.name);
        pivot = ChangePivot(solarRoot, planet.position); 
        
        _startPivotPos = pivot.position;
        
        focusIn = true;
        StartCoroutine(ZoomInRoutine());
    }

    void Update()
    {
        if (solarRoot.lossyScale.x >= modelAppearScale && !showModel)
        {
            planetVisual.ShowModel();
            _startPivotPos = pivot.position;
            showModel = true;
        }
        else if (solarRoot.lossyScale.x < modelAppearScale && showModel)
        {
            planetVisual.ShowMarker();
            showModel = false;
            
            if (planetVisual.planetName == "Sun") return;
            planetSelectable.ResetFocus();
        }

        if (pivot.lossyScale.x >= questionAppearScale && !showInfor)
        {
            if (planetVisual.planetName == "Sun") return;
            quizAndInforManager.ShowPanel(planetVisual.planetName);
            showInfor = true;
        }
        else if (pivot.lossyScale.x < questionAppearScale && showInfor)
        {
            if (planetVisual.planetName == "Sun") return;
            quizAndInforManager.HidePanel(planetVisual.planetName);
            showInfor = false;
        }
    }

    public void ZoomOut()
    {
        focusOut = true;
        focusIn = false;
        StartCoroutine(ZoomOutRoutine());
        
        // BẬT LẠI QUỸ ĐẠO khi zoom out
        if (planetVisual != null)
        {
            PlanetVisual bigVisual = planetController.bigPlanets.Find(p => p.planetName == planetVisual.planetName);
            if (bigVisual != null)
            {
                PlanetOrbit[] orbits = bigVisual.GetComponentsInParent<PlanetOrbit>();
                foreach (var o in orbits) o.enabled = true;
            }
        }
    }

    private IEnumerator ZoomInRoutine()
    {
        float elapsed = 0f;
        float currentT = (pivot.localScale.x - minScale) / (targetScale - minScale);
        if (float.IsNaN(currentT) || float.IsInfinity(currentT)) currentT = 0f;
        currentT = Mathf.Clamp01(currentT);
        
        float startT = currentT;
        float duration = zoomSpeed * (1f - currentT);

        while (elapsed < duration && focusIn)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = t * t * (3f - 2f * t);

            float mappedT = Mathf.Lerp(startT, 1f, t);
            SetSystemScale(mappedT * 100f);
            handle.UpdateHandleByScale(pivot.localScale.x);
            
            yield return null;
        }

        SetSystemScale(100f);
        handle.UpdateHandleByScale(targetScale);
    }
    
    private IEnumerator ZoomOutRoutine()
    {
    float elapsed = 0f;
        float currentT = (pivot.localScale.x - minScale) / (targetScale - minScale);
        if (float.IsNaN(currentT) || float.IsInfinity(currentT)) currentT = 1f;
        currentT = Mathf.Clamp01(currentT);

        float startT = currentT;
        float duration = zoomSpeed * currentT;

        while (elapsed < duration && focusOut)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = t * t * (3f - 2f * t);

            float mappedT = Mathf.Lerp(startT, 0f, t);
            SetSystemScale(mappedT * 100f);
            handle.UpdateHandleByScale(pivot.localScale.x);
            
            yield return null;
        }

        SetSystemScale(0f);
        handle.UpdateHandleByScale(minScale);
        

        // Reset flag để lần zoom sau hoạt động bình thường
        showModel = false;
        showInfor = false;

        if (PlanetRotator.Instance != null)
            PlanetRotator.Instance.ClearPlanet();
    }
    
    public Transform ChangePivot(Transform objectToMove, Vector3 newPivotPosition)
    {
        Transform pivotTransform;

        // 1. Kiểm tra xem object đã nằm trong một SystemPivot chưa
        bool alreadyHasPivot = objectToMove.parent != null && objectToMove.parent.name=="SystemPivot"; 
        // Mẹo: Nên dùng Tag thay vì Name để check nhanh hơn

        if (alreadyHasPivot)
        {
            pivotTransform = objectToMove.parent;
            // Tách ra tạm thời để tính toán vị trí mới của pivot mà không kéo theo object
            objectToMove.SetParent(pivotTransform.parent, true);
        }
        else
        {
            GameObject pivotObj = new GameObject("SystemPivot");
            // pivotObj.tag = "SystemPivot"; // Nếu bạn dùng Tag
            pivotTransform = pivotObj.transform;
            pivotTransform.SetParent(objectToMove.parent, true);
        }

        // 2. Cập nhật vị trí và xoay cho Pivot
        pivotTransform.position = newPivotPosition;
    
        // Giữ nguyên Rotation hiện tại của Object để tránh bị giật (snap) khi gán cha
        pivotTransform.rotation = objectToMove.rotation;
    
        // 3. Gắn Object lại vào Pivot mới
        objectToMove.SetParent(pivotTransform, true);

        return pivotTransform;
    }
    
}