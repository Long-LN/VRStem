using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(PlanetVisual))]
public class PlanetSelectable : MonoBehaviour
{
    public OvalRing orbit;
    public SolarSystemFocus focusSystem;
    public float originScale;
    public PlanetVisual visual;

    private void Awake()
    {
        originScale = transform.localScale.x;
        visual = GetComponent<PlanetVisual>();
    }

    void Start()
    {
        GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>().selectEntered.AddListener(OnSelect);
    }

    void OnSelect(SelectEnterEventArgs args)
    {
        Debug.Log(gameObject.name);
        focusSystem.FocusPlanet(transform, visual);
    }

    private void Update()
    {
        //Scale
        float orbitRadius = orbit.GetRadius();
        float size = orbitRadius * originScale * 0.1f;
        transform.localScale = Vector3.one * size;
        
        transform.position = orbit.GetPoint(0);
        if(orbit.radiusX == 77.7f)
            Debug.DrawLine(orbit.transform.position, orbit.GetPoint(0), Color.red);
    }
}