using System;
using System.Collections;
using UnityEngine;

public class SolarSystemFocus : MonoBehaviour
{
    public static SolarSystemFocus Instance;
    public Transform solarRoot;

    public PlanetController planetController;

    public float zoomSpeed = 2f;
    public float targetScale = 100f;
    public float modelAppearScale = 10f;
    public float quizDelay = 3f;

    [Header("Planet Groups")]
    public Transform planetGroupBig; // Kéo PlanetGroup (1) vào đây

    [Header("Sun")]
    public GameObject sun; // Kéo Sun vào đây trong Inspector

    public Transform pivot;
    bool focusing;

    PlanetVisual smallPlanetVisual;  // Visual của hành tinh NHỎ
    PlanetVisual bigPlanetVisual;    // Visual của hành tinh TO
    PlanetSelectable currentPlanetSelectable;
    string currentPlanetName;

    public XRScaleKnobDelta scaleKnob;

    private void Awake()
    {
        Instance = this;
        pivot = transform;
    }

    public void FocusPlanet(Transform planet, PlanetVisual visual)
    {
        smallPlanetVisual = visual;
        currentPlanetSelectable = planet.GetComponent<PlanetSelectable>();
        currentPlanetName = visual.planetName;

        // Tìm PlanetVisual của hành tinh TO tương ứng
        bigPlanetVisual = FindBigPlanetVisual(currentPlanetName);

        planetController.SetPlanetZoom(visual);
        pivot = ChangePivot(solarRoot, planet.position);
        scaleKnob.ChangePivot(pivot);
        focusing = true;

        // Ẩn tất cả vòng quỹ đạo, hành tinh và mặt trời khi zoom vào
        SetAllRingsVisible(false);
        SetAllPlanetsVisible(false);
        if (sun != null) sun.SetActive(false);

        // Giữ lại hành tinh đang được focus bằng SetVisible
        PlanetSelectable focusedPlanet = planet.GetComponent<PlanetSelectable>();
        if (focusedPlanet != null)
            focusedPlanet.SetVisible(true);

        Debug.Log(planet.name);
    }

    // Tìm PlanetVisual trong PlanetGroup (1) theo tên
    private PlanetVisual FindBigPlanetVisual(string name)
    {
        if (planetGroupBig == null) return null;
        foreach (Transform child in planetGroupBig)
        {
            if (child.name == name)
                return child.GetComponent<PlanetVisual>();
        }
        return null;
    }

    // Bật/tắt tất cả vòng quỹ đạo
    private void SetAllRingsVisible(bool visible)
    {
        foreach (var ring in FindObjectsOfType<OvalRing>())
            ring.SetRingVisible(visible);
    }

    // Bật/tắt tất cả hành tinh nhỏ
    private void SetAllPlanetsVisible(bool visible)
    {
        foreach (var planet in FindObjectsOfType<PlanetSelectable>())
            planet.SetVisible(visible);
    }

    void Update()
    {
        if (!focusing) return;

        float scale = Mathf.Lerp(
            pivot.localScale.x,
            targetScale,
            Time.deltaTime * zoomSpeed
        );

        pivot.localScale = Vector3.one * scale;

        if (scale >= modelAppearScale)
            smallPlanetVisual.ShowModel();
        else
            smallPlanetVisual.ShowMarker();

        if (Mathf.Abs(scale - targetScale) < 0.01f)
        {
            focusing = false;

            if (currentPlanetSelectable != null)
                currentPlanetSelectable.ResetFocus();

            bool alreadyAnswered = PlanetQuiz.Instance != null &&
                                   PlanetQuiz.Instance.IsAnswered(currentPlanetName);

            if (alreadyAnswered)
            {
                Debug.Log($"[SolarSystemFocus] Hiện InfoPanel cho {currentPlanetName}");
                if (bigPlanetVisual != null)
                    bigPlanetVisual.ShowInfo();
                else
                    Debug.LogWarning($"[SolarSystemFocus] Không tìm thấy bigPlanetVisual cho {currentPlanetName}!");
            }
            else
            {
                Debug.Log($"[SolarSystemFocus] Chưa trả lời {currentPlanetName} → hiện quiz");
                StartCoroutine(ShowQuizAfterDelay(currentPlanetName, smallPlanetVisual, quizDelay));
            }
        }
    }

    private IEnumerator ShowQuizAfterDelay(string planetName, PlanetVisual visual, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (PlanetQuiz.Instance != null)
            PlanetQuiz.Instance.StartQuiz(planetName, visual);
    }

    public void ZoomOut()
    {
        if (pivot == null) return;

        if (bigPlanetVisual != null)
            bigPlanetVisual.HideInfo();

        // Hiện lại tất cả vòng quỹ đạo, hành tinh và mặt trời khi zoom out
        SetAllRingsVisible(true);
        SetAllPlanetsVisible(true);
        if (sun != null) sun.SetActive(true);

        StartCoroutine(ZoomOutRoutine());
    }

    private IEnumerator ZoomOutRoutine()
    {
        float duration = 2f;
        float elapsed = 0f;
        float startScale = pivot.localScale.x;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = t * t * (3f - 2f * t);
            pivot.localScale = Vector3.one * Mathf.Lerp(startScale, 1f, t);
            yield return null;
        }

        pivot.localScale = Vector3.one;
        smallPlanetVisual.ShowMarker();

        Debug.Log("[SolarSystemFocus] Zoom OUT hoàn tất!");
    }

    public Transform ChangePivot(Transform objectToMove, Vector3 newPivotPosition)
    {
        Transform pivot;

        if (objectToMove.parent != null && objectToMove.parent.name.Contains("_Pivot"))
        {
            pivot = objectToMove.parent;
            Transform oldParent = pivot.parent;
            objectToMove.SetParent(oldParent);
            pivot.position = newPivotPosition;
            pivot.rotation = objectToMove.rotation;
            objectToMove.SetParent(pivot);
        }
        else
        {
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