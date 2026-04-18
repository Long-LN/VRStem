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
    
    [Header("UI Khung Slider")]
    public GameObject sliderPanel; // [THÊM MỚI] Chứa toàn bộ khung xám và text
    
    public float zoomSpeed = 1f;
    public float minScale;
    public float slowThreshold = 0.2f;
    public float targetScale = 100f;
    public float modelAppearScale = 1f;
    public float questionAppearScale = 95f;
    
    private bool showModel = false;
    private bool showInfor = false;

    public Transform pivot;
    public bool focusIn;
    public bool focusOut;

    [FormerlySerializedAs("planetVisual")] public PlanetVisual currentPlanetVisual;
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
        float t = progress / 100f;
        t = Mathf.SmoothStep(0, 1, t);
        float newScale = Mathf.Lerp(minScale, targetScale, t);

        if (pivot != null)
        {
            pivot.localScale = Vector3.one * newScale;

            if(_startPivotPos != Vector3.zero)
                pivot.position = Vector3.Lerp(_startPivotPos, _targetPivotPos, t);
        }
    }

    public void FocusPlanet(Transform planet, PlanetVisual visual)
    {
        currentPlanetVisual = visual;
        planetSelectable = planet.GetComponent<PlanetSelectable>();
        planetController.SetPlanetZoom(visual);
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
            currentPlanetVisual.ShowModel();
            _startPivotPos = pivot.position;
            showModel = true;
            planetSelectable.orbit.SetRingVisible(false);
        }
        else if (solarRoot.lossyScale.x < modelAppearScale && showModel)
        {
            currentPlanetVisual.ShowMarker();
            showModel = false;
            
            if (currentPlanetVisual.planetName == "Sun") return;
            planetSelectable.ResetFocus();
            planetSelectable.orbit.SetRingVisible(true);
        }

        if (pivot.lossyScale.x >= questionAppearScale && !showInfor)
        {
            if (currentPlanetVisual.planetName == "Sun") return;
            quizAndInforManager.ShowPanel(currentPlanetVisual.planetName);
            showInfor = true;
        }
        else if (pivot.lossyScale.x < questionAppearScale && showInfor)
        {
            if (currentPlanetVisual.planetName == "Sun") return;
            quizAndInforManager.HidePanel(currentPlanetVisual.planetName);
            showInfor = false;
        }
    }

    public void ZoomOut()
    {
        focusOut = true;
        focusIn = false;
        StartCoroutine(ZoomOutRoutine());
        
        if (currentPlanetVisual != null)
        {
            PlanetVisual bigVisual = planetController.bigPlanets.Find(p => p.planetName == currentPlanetVisual.planetName);
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
            float normalizedTime = elapsed / duration;
            float t = Mathf.SmoothStep(0, 1, normalizedTime);
        
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
            float normalizedTime = elapsed / duration;

            float t = Mathf.SmoothStep(0, 1, normalizedTime);

            float mappedT = Mathf.Lerp(startT, 0f, t);
            SetSystemScale(mappedT * 100f);
            handle.UpdateHandleByScale(pivot.localScale.x);
            
            yield return null;
        }

        SetSystemScale(0f);
        handle.UpdateHandleByScale(minScale);
        
        showModel = false;
        showInfor = false;

        if (PlanetRotator.Instance != null)
            PlanetRotator.Instance.ClearPlanet();
    }
    
    public Transform ChangePivot(Transform objectToMove, Vector3 newPivotPosition)
    {
        Transform pivotTransform;

        bool alreadyHasPivot = objectToMove.parent != null && objectToMove.parent.name=="SystemPivot"; 

        if (alreadyHasPivot)
        {
            pivotTransform = objectToMove.parent;
            objectToMove.SetParent(pivotTransform.parent, true);
        }
        else
        {
            GameObject pivotObj = new GameObject("SystemPivot");
            pivotTransform = pivotObj.transform;
            pivotTransform.SetParent(objectToMove.parent, true);
        }

        pivotTransform.position = newPivotPosition;
        pivotTransform.rotation = objectToMove.rotation;
        objectToMove.SetParent(pivotTransform, true);

        return pivotTransform;
    }
}