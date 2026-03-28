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
    public float minScale = 0.5f;
    public float targetScale = 100f;
    public float modelAppearScale = 1f;
    public float questionAppearScale = 95f;
    
    private bool showModel = false;
    private bool showInfor = false;

    public Transform pivot;
    public bool focusIn;
    public bool focusOut;

    //Bat dau se mac dinh la sun
    public PlanetVisual planetVisual;
    public PlanetSelectable planetSelectable;

    private void Awake()
    {
        Instance = this;
        pivot = solarRoot;
    }
    
    public void SetSystemScale(float progress)
    {
        // Debug.Log(progress);
        // Chuyển đổi progress từ (0-100) sang (0-1)
        float t = progress / 100f;

        // Nội suy giá trị scale dựa trên progress
        float newScale = Mathf.Lerp(minScale, targetScale, t);

        // Áp dụng cho solarRoot hoặc pivot hiện tại
        if (pivot != null)
        {
            pivot.localScale = Vector3.one * newScale;
        }
        
        // Cập nhật trạng thái hiển thị Model/Marker dựa trên scale mới
        // UpdateVisuals(newScale);
    }

    public void FocusPlanet(Transform planet, PlanetVisual visual)
    {
        planetVisual = visual;
        planetSelectable = planet.GetComponent<PlanetSelectable>();
        planetController.SetPlanetZoom(visual);
        // dùng hàm ChangePivot
        Debug.Log(planet.name);
        pivot = ChangePivot(solarRoot, planet.position);
        focusIn = true;
        StartCoroutine(ZoomInRoutine());

        // Set hành tinh TO cho PlanetRotator
        if (PlanetRotator.Instance != null)
        {
            PlanetVisual bigVisual = planetController.bigPlanets.Find(p => p.planetName == visual.planetName);
            if (bigVisual != null)
                PlanetRotator.Instance.SetPlanet(bigVisual.model);
        }
    }

    void Update()
    {
        //Check scale
        // Debug.Log(solarRoot.localScale + " " + modelAppearScale + " " + solarRoot.lossyScale);
        if (solarRoot.lossyScale.x >= modelAppearScale && !showModel)
        {
            planetVisual.ShowModel();
            showModel = true;
        }
        else if (solarRoot.lossyScale.x < modelAppearScale && showModel)
        {
            planetVisual.ShowMarker();
            showModel = false;
            
            if (planetVisual.planetName == "Sun") return;
            planetSelectable.ResetFocus();
        }

        //Check scale để quyết điịnh c hiện câu hỏi hay ko nữa
        // Debug.Log(pivot.lossyScale.x + " - " + showInfor);
        if (pivot.lossyScale.x >= questionAppearScale && !showInfor)
        {
            if (planetVisual.planetName == "Sun") return;
            Debug.Log("ShowPanel");
            quizAndInforManager.ShowPanel(planetVisual.planetName);
            showInfor = true;
        }
        else if (pivot.lossyScale.x < questionAppearScale && showInfor)
        {
            if (planetVisual.planetName == "Sun") return;
            Debug.Log("HidePanel");
            quizAndInforManager.HidePanel(planetVisual.planetName);
            
            showInfor = false;
        }
        
    }

    public void ZoomOut()
    {
        focusOut = true;
        StartCoroutine(ZoomOutRoutine());
    }

    private IEnumerator ZoomInRoutine()
    {
        float elapsed = 0f;
        float startScale = pivot.localScale.x;

        while (elapsed < zoomSpeed && focusIn)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / zoomSpeed;
            t = t * t * (3f - 2f * t);

            float scale = Mathf.Lerp(startScale, targetScale, t);
            pivot.localScale = Vector3.one * scale;

            handle.UpdateHandleByScale(scale);
            yield return null;
        }
        
    }
    
    private IEnumerator ZoomOutRoutine()
    {
        float elapsed = 0f;
        float startScale = pivot.localScale.x;

        while (elapsed < zoomSpeed && focusOut)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / zoomSpeed;
            t = t * t * (3f - 2f * t);
            float scale = Mathf.Lerp(startScale, minScale, t);
            pivot.localScale = Vector3.one * scale;
            handle.UpdateHandleByScale(scale);
            yield return null;
        }

        // Xóa hành tinh khỏi PlanetRotator
        if (PlanetRotator.Instance != null)
            PlanetRotator.Instance.ClearPlanet();
        
        Debug.Log("[SolarSystemFocus] Zoom OUT hoàn tất!");
    }

    public Transform ChangePivot(Transform objectToMove, Vector3 newPivotPosition)
    {
        Transform pivot;

        // nếu đã có pivot
        if (objectToMove.parent != null && objectToMove.parent.name.Contains("_Pivot"))
        {
            pivot = objectToMove.parent;

            Transform oldParent = pivot.parent;

            // tạm tháo object ra
            objectToMove.SetParent(oldParent);

            // di chuyển pivot
            pivot.position = newPivotPosition;
            pivot.rotation = objectToMove.rotation;

            // gắn lại object vào pivot
            objectToMove.SetParent(pivot);
        }
        else
        {
            // Create new pivot object (giữ nguyên logic cũ)
            GameObject pivotObj = new GameObject(objectToMove.name + "_Pivot");
            pivot = pivotObj.transform;

            pivot.position = newPivotPosition;
            pivot.rotation = objectToMove.rotation;

            Transform oldParent = objectToMove.parent;

            if (oldParent != null)
                pivot.SetParent(oldParent);

            objectToMove.SetParent(pivot);
        }

        return pivot;
    }
}