using System;
using UnityEngine;

public class SolarSystemFocus : MonoBehaviour
{
    public static SolarSystemFocus Instance;
    public Transform solarRoot;

    public float zoomSpeed = 2f;
    public float targetScale = 100f;
    public float modelAppearScale = 10f;

    public Transform pivot;
    bool focusing;

    PlanetVisual planetVisual;
    
    public XRScaleKnobDelta scaleKnob;

    private void Awake()
    {
        Instance = this;
        pivot = transform;
    }

    public void FocusPlanet(Transform planet, PlanetVisual visual)
    {
        planetVisual = visual;

        // dùng hàm ChangePivot
        Debug.Log(planet.name);
        pivot = ChangePivot(solarRoot, planet.position);
        scaleKnob.ChangePivot(pivot);
        focusing = true;
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
        
        // Debug.Log(scale);

        if (scale >= modelAppearScale)
            planetVisual.ShowModel();
        else
            planetVisual.ShowMarker();

        if (Mathf.Abs(scale - targetScale) < 0.01f)
            focusing = false;
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