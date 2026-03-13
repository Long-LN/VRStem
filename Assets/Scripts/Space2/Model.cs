using System;
using UnityEngine;

public class Model : MonoBehaviour
{
    public float targetScale;
    public float currentScale;
    
    public float orbitScale;
    public float orbitStartScale;
    public float orbitEndScale;
    
    
    private void Awake()
    {
        targetScale = transform.localScale.x;
        orbitScale = SolarSystemFocus.Instance.pivot.localScale.x;
        orbitStartScale = SolarSystemFocus.Instance.modelAppearScale;
        orbitEndScale = SolarSystemFocus.Instance.targetScale;
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(SolarSystemFocus.Instance.pivot.localScale.x);
        currentScale = ((SolarSystemFocus.Instance.pivot.localScale.x - orbitStartScale) / orbitEndScale) * targetScale;
        transform.localScale = currentScale * Vector3.one;
    }
}
