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
    }

    void Start()
    {
        // Dùng Start thay Awake để đảm bảo SolarSystemFocus đã khởi tạo xong
        if (SolarSystemFocus.Instance != null && SolarSystemFocus.Instance.pivot != null)
        {
            orbitScale = SolarSystemFocus.Instance.pivot.localScale.x;
            orbitStartScale = SolarSystemFocus.Instance.modelAppearScale;
            orbitEndScale = SolarSystemFocus.Instance.targetScale;
        }
    }

    void Update()
    {
        if (SolarSystemFocus.Instance == null || SolarSystemFocus.Instance.pivot == null) return;
        if (orbitEndScale == 0) return; // Tránh chia cho 0

        currentScale = ((SolarSystemFocus.Instance.pivot.localScale.x - orbitStartScale) / orbitEndScale) * targetScale;
        transform.localScale = currentScale * Vector3.one;
    }
}