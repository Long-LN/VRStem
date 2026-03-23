using UnityEngine;

/// <summary>
/// Gắn vào từng hành tinh trong PlanetGroup nhỏ
/// Hành tinh sẽ xoay theo đúng quỹ đạo OvalRing
/// </summary>
public class PlanetOrbit : MonoBehaviour
{
    [Header("Quỹ đạo")]
    public OvalRing orbit;             // Kéo MercuryOrbit, VenusOrbit... vào đây
    public float orbitSpeed = 20f;     // Độ/giây xoay quanh quỹ đạo
    public float selfRotateSpeed = 50f; // Tốc độ tự xoay

    private float currentAngle = 0f;

    private void Start()
    {
        // Random góc ban đầu để các hành tinh không xếp hàng
        currentAngle = Random.Range(0f, Mathf.PI * 2f);

        // Đặt vị trí ban đầu đúng trên quỹ đạo
        if (orbit != null)
            transform.position = orbit.GetPoint(currentAngle);
    }

    private void Update()
    {
        if (orbit == null) return;

        // Tăng góc theo thời gian
        currentAngle += orbitSpeed * Mathf.Deg2Rad * Time.deltaTime;
        if (currentAngle > Mathf.PI * 2f)
            currentAngle -= Mathf.PI * 2f;

        // Đặt vị trí hành tinh đúng trên đường ellipse
        transform.position = orbit.GetPoint(currentAngle);

        // Tự xoay quanh trục
        transform.Rotate(Vector3.up, selfRotateSpeed * Time.deltaTime, Space.Self);
    }
}