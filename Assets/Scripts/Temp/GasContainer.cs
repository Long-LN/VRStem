using UnityEngine;

public class GasContainer : MonoBehaviour
{
    public float wallBoost = 1.05f;
    public float maxSpeed = 5f;
    public float minSpeed = 0.5f;

    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Molecule")) return;
        if (collision.contacts.Length == 0) return;

        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
        if (rb == null) return;

        Vector3 normal = collision.contacts[0].normal;
        Vector3 incoming = rb.linearVelocity;
        float currentSpeed = Mathf.Max(incoming.magnitude, minSpeed);

        Vector3 reflected = Vector3.Reflect(incoming, normal);
        if (reflected.sqrMagnitude < 0.01f || Vector3.Dot(reflected, normal) < 0.1f)
            reflected = normal + Random.insideUnitSphere * 0.3f;

        float newSpeed = Mathf.Clamp(currentSpeed * wallBoost, minSpeed, maxSpeed);
        rb.linearVelocity = reflected.normalized * newSpeed;
    }

    void OnCollisionStay(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Molecule")) return;
        if (collision.contacts.Length == 0) return;

        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
        if (rb == null) return;

        Vector3 normal = Vector3.zero;
        foreach (var c in collision.contacts) normal += c.normal;
        normal.Normalize();

        rb.linearVelocity = (normal + Random.insideUnitSphere * 0.3f).normalized * minSpeed;
    }
}