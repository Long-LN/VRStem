using UnityEngine;
using System.Collections;

public class MoleculeFloat : MonoBehaviour
{
    [Header("Movement")]
    public float brownianForce = 0.05f;
    public float maxSpeed = 0.3f;
    public float moveRadius = 1.5f;

    [Header("Collision")]
    public float collisionBoost = 1.3f;
    public float maxSpeedCap = 5f;

    [Header("Stuck Prevention")]
    public float stuckCheckInterval = 0.2f;   // giảm từ 0.5 xuống 0.2 để kick nhanh hơn
    public float stuckSpeedThreshold = 0.1f;

    Rigidbody rb;
    Vector3 spawnCenter;
    bool confined = false;
    float savedSpeed = 2f;

    // Thêm 6 hướng thẳng trục để thoát khỏi sàn/tường/trần
    static readonly Vector3[] fixedDirections = new Vector3[]
    {
        new Vector3( 0,  1,  0),   // thẳng lên
        new Vector3( 0, -1,  0),   // thẳng xuống
        new Vector3( 1,  0,  0),   // thẳng phải
        new Vector3(-1,  0,  0),   // thẳng trái
        new Vector3( 0,  0,  1),   // thẳng ra
        new Vector3( 0,  0, -1),   // thẳng vào
        new Vector3( 1,  1,  0).normalized,
        new Vector3(-1,  1,  0).normalized,
        new Vector3( 0,  1,  1).normalized,
        new Vector3( 0,  1, -1).normalized,
        new Vector3( 1,  0,  1).normalized,
        new Vector3(-1,  0,  1).normalized,
        new Vector3( 1, -1,  0).normalized,
        new Vector3(-1, -1,  0).normalized,
        new Vector3( 0, -1,  1).normalized,
        new Vector3( 0, -1, -1).normalized,
        new Vector3( 1,  0, -1).normalized,
        new Vector3(-1,  0, -1).normalized,
    };

    int directionIndex = 0;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.linearDamping = 0f;
        rb.angularDamping = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        spawnCenter = transform.position;
        rb.linearVelocity = Vector3.zero;

        InvokeRepeating(nameof(StuckCheck), stuckCheckInterval, stuckCheckInterval);
    }

    void StuckCheck()
    {
        if (rb == null) return;
        if (!confined) return;
        if (rb.isKinematic) return;

        if (rb.linearVelocity.magnitude < stuckSpeedThreshold)
        {
            // Dùng random hoàn toàn thay vì tuần tự để tránh kick vào tường liên tục
            Vector3 dir = Random.insideUnitSphere.normalized;

            // Fallback nếu random ra vector quá nhỏ
            if (dir.sqrMagnitude < 0.01f)
                dir = fixedDirections[directionIndex % fixedDirections.Length];

            directionIndex++;
            float speed = Mathf.Max(savedSpeed, 1f);
            rb.linearVelocity = dir * speed;
        }
    }

    public float GetSavedSpeed() => savedSpeed;

    public void ActivateInBox(float speed)
    {
        StartCoroutine(DelayActivate(speed));
    }

    IEnumerator DelayActivate(float speed)
    {
        yield return null;
        yield return null;

        if (rb == null) rb = GetComponent<Rigidbody>();

        rb.isKinematic = false;
        rb.useGravity = false;
        rb.linearDamping = 0f;
        rb.angularDamping = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

        SetConfined();
        enabled = true;
        savedSpeed = speed;

        Vector3 dir = fixedDirections[directionIndex % fixedDirections.Length];
        directionIndex++;
        rb.linearVelocity = dir * speed;
    }

    public void SetConfined() => confined = true;
    public bool IsConfined() => confined;

    void FixedUpdate()
    {
        if (rb == null) return;
        if (rb.isKinematic) return;

        if (!confined)
        {
            rb.AddForce(Random.insideUnitSphere * brownianForce, ForceMode.Acceleration);

            float dist = Vector3.Distance(transform.position, spawnCenter);
            if (dist > moveRadius)
            {
                Vector3 pullDir = (spawnCenter - transform.position).normalized;
                rb.AddForce(pullDir * brownianForce * 4f, ForceMode.Acceleration);
            }

            if (rb.linearVelocity.magnitude > maxSpeed)
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
        else
        {
            if (rb.linearVelocity.sqrMagnitude > 0.001f)
                savedSpeed = rb.linearVelocity.magnitude;

            if (rb.linearVelocity.magnitude > maxSpeedCap)
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeedCap;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (rb == null) return;
        if (rb.isKinematic) return;
        if (!collision.gameObject.CompareTag("Molecule")) return;

        TempAudioManager.Instance?.PlaySFX(
            TempAudioManager.Instance.moleculeHitSound, 0.3f);

        float newSpeed = Mathf.Min(
            rb.linearVelocity.magnitude * collisionBoost,
            maxSpeedCap
        );
        if (rb.linearVelocity.sqrMagnitude > 0.001f)
            rb.linearVelocity = rb.linearVelocity.normalized * newSpeed;
    }
}