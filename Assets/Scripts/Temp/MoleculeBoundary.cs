using UnityEngine;

public class MoleculeBoundary : MonoBehaviour
{
    public BoxCollider boxCollider;

    [Header("Chỉnh bounds cho khớp hộp")]
    public Vector3 halfSizeOverride = Vector3.zero; // set > 0 để override
    public Vector3 centerOverride = Vector3.zero;

    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        enabled = false;
    }

    void FixedUpdate()
    {
        if (rb == null) return;
        if (rb.isKinematic) return;
        if (boxCollider == null) return;

        // Dùng override nếu có, không thì tự tính
        Vector3 half = halfSizeOverride != Vector3.zero
            ? halfSizeOverride
            : boxCollider.size * 0.48f;

        Vector3 center = centerOverride != Vector3.zero
            ? centerOverride
            : boxCollider.center;

        Vector3 localPos = boxCollider.transform.InverseTransformPoint(rb.position) - center;
        Vector3 localVel = boxCollider.transform.InverseTransformDirection(rb.linearVelocity);
        bool hit = false;

        if (localPos.x >  half.x) { localPos.x =  half.x; localVel.x = -Mathf.Abs(localVel.x); hit = true; }
        if (localPos.x < -half.x) { localPos.x = -half.x; localVel.x =  Mathf.Abs(localVel.x); hit = true; }
        if (localPos.y >  half.y) { localPos.y =  half.y; localVel.y = -Mathf.Abs(localVel.y); hit = true; }
        if (localPos.y < -half.y) { localPos.y = -half.y; localVel.y =  Mathf.Abs(localVel.y); hit = true; }
        if (localPos.z >  half.z) { localPos.z =  half.z; localVel.z = -Mathf.Abs(localVel.z); hit = true; }
        if (localPos.z < -half.z) { localPos.z = -half.z; localVel.z =  Mathf.Abs(localVel.z); hit = true; }

        if (hit)
        {
            rb.position = boxCollider.transform.TransformPoint(localPos + center);
            rb.linearVelocity = boxCollider.transform.TransformDirection(localVel);
        }
    }

    void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (boxCollider == null) return;
        Vector3 half = halfSizeOverride != Vector3.zero
            ? halfSizeOverride
            : boxCollider.size * 0.48f;
        Vector3 center = centerOverride != Vector3.zero
            ? centerOverride
            : boxCollider.center;

        Gizmos.color = Color.yellow;
        Gizmos.matrix = boxCollider.transform.localToWorldMatrix;
        Gizmos.DrawWireCube(center, half * 2f);
#endif
    }
}