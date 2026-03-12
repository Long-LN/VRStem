using System;
using UnityEngine;

public class Model : MonoBehaviour
{
    public float targetScale;
    public float currentScale;
    
    public float orbitScale;
    public float orbitStartScale;
    public float orbitEndScale;
    
    public float distanceScale;
    
    private void Awake()
    {
        orbitScale = SolarSystemFocus.Instance.pivot.localScale.x;
        orbitStartScale = SolarSystemFocus.Instance.modelAppearScale;
        orbitEndScale = SolarSystemFocus.Instance.targetScale;
    }

    // Update is called once per frame
    void Update()
    {
        currentScale = ((orbitScale - orbitStartScale) / orbitEndScale) * targetScale;
        transform.localScale = currentScale * Vector3.one;
    }
}
