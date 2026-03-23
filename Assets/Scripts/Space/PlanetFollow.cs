using UnityEngine;

public class PlanetFollow : MonoBehaviour
{
    private LineRenderer orbitLine;
    public float orbitSpeed = 5f;

    [Header("Rotation")]
    public float rotationSpeed = 20f;

    [Header("Axial Tilt")]
    public float axialTilt = 23.44f;

    private Vector3 tiltAxis;
    
    private float t;
    private PlanetSelectable planet;

    void Awake()
    {
        planet = GetComponent<PlanetSelectable>();
    }

    void Start()
    {
        orbitLine = planet.orbit.GetComponent<LineRenderer>();
        // nghiêng trục
        transform.localRotation = Quaternion.Euler(axialTilt, 0, 0);
        // trục quay sau khi nghiêng
        tiltAxis = transform.up;
    }

    void Update()
    {
        int count = orbitLine.positionCount;
        if (count < 2) return;

        t += orbitSpeed * Time.deltaTime;

        int current = Mathf.FloorToInt(t) % count;
        int next = (current + 1) % count;

        float lerp = t - Mathf.Floor(t);

        // lấy point local từ LineRenderer
        Vector3 localA = orbitLine.GetPosition(current);
        Vector3 localB = orbitLine.GetPosition(next);

        // convert sang world (vì orbit dùng local space)
        Vector3 worldA = orbitLine.transform.TransformPoint(localA);
        Vector3 worldB = orbitLine.transform.TransformPoint(localB);

        transform.position = Vector3.Lerp(worldA, worldB, lerp);
        transform.Rotate(tiltAxis, rotationSpeed * Time.deltaTime, Space.World);

    }
}