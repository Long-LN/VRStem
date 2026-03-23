using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(PlanetVisual))]
public class PlanetSelectable : MonoBehaviour
{
    public OvalRing orbit;
    private SolarSystemFocus focusSystem;
    private PlanetVisual visual;
    public float originScale;

    [Header("Orbit Settings")]
    public float orbitSpeed = 20f; // Tốc độ xoay quanh mặt trời (độ/giây)

    private float currentAngle = 0f;
    private bool isFocusing = false;
    private bool isHidden = false;

    private void Awake()
    {
        originScale = transform.localScale.x;
        visual = GetComponent<PlanetVisual>();
        // Random góc ban đầu để các hành tinh không xếp hàng
        currentAngle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
    }

    void Start()
    {
        focusSystem = SolarSystemFocus.Instance;
        GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>()
            .selectEntered.AddListener(OnSelect);
    }

    void OnSelect(SelectEnterEventArgs args)
    {
        Debug.Log(gameObject.name);
        isFocusing = true;
        focusSystem.FocusPlanet(transform, visual);
    }

    public void ResetFocus()
    {
        isFocusing = false;
    }

    // Ẩn/hiện hành tinh bằng scale thay vì SetActive
    public void SetVisible(bool visible)
    {
        isHidden = !visible;
        transform.localScale = visible ? Vector3.one * originScale : Vector3.zero;
    }

    private void Update()
    {
        if (isFocusing) return;
        if (isHidden) return;

        // Scale theo bán kính quỹ đạo
        float orbitRadius = orbit.GetRadius();
        float size = orbitRadius * originScale * 0.1f;
        transform.localScale = Vector3.one * size;

        // Tăng góc theo thời gian → hành tinh di chuyển quanh mặt trời
        currentAngle += orbitSpeed * Mathf.Deg2Rad * Time.deltaTime;
        if (currentAngle > Mathf.PI * 2f)
            currentAngle -= Mathf.PI * 2f;

        // Đặt vị trí theo đường ellipse
        transform.position = orbit.GetPoint(currentAngle);

        if (orbit.radiusX == 77.7f)
            Debug.DrawLine(orbit.transform.position, orbit.GetPoint(currentAngle), Color.red);
    }
}