using UnityEngine;

public class KeepRotation : MonoBehaviour
{
    private Quaternion fixedRotation;
    private Transform planetTransform; // Transform của planet cha
    private Vector3 offsetFromPlanet;  // Khoảng cách ban đầu từ planet tới panel

    private void Start()
    {
        // Lưu rotation ban đầu (luôn nhìn thẳng)
        fixedRotation = transform.rotation;

        // Lấy planet cha (parent của InfoPanel)
        planetTransform = transform.parent;

        // Tính offset ban đầu trong world space
        // (ví dụ: panel ở dưới planet 1 đơn vị)
        offsetFromPlanet = transform.position - planetTransform.position;
    }

    private void LateUpdate()
    {
        if (planetTransform == null) return;

        // Giữ panel luôn ở đúng vị trí bên dưới planet (không quay theo)
        transform.position = planetTransform.position + offsetFromPlanet;

        // Giữ rotation cố định
        transform.rotation = fixedRotation;
    }
}