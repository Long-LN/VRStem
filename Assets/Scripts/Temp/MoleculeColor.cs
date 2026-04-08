using UnityEngine;

public class MoleculeColor : MonoBehaviour
{
    Renderer rend;
    Rigidbody rb;
    Material mat;

    public float maxSpeed = 5f;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        rb = GetComponent<Rigidbody>();
        mat = new Material(rend.material);
        rend.material = mat;
    }

    void Update()
    {
        if (rb == null) return;

        float t = Mathf.Clamp01(rb.linearVelocity.magnitude / maxSpeed);

        Color cold = new Color(0.2f, 0.4f, 1f);
        Color warm = new Color(1f, 0.6f, 0f);
        Color hot  = new Color(1f, 0.1f, 0f);

        Color current = t < 0.5f
            ? Color.Lerp(cold, warm, t * 2f)
            : Color.Lerp(warm, hot, (t - 0.5f) * 2f);

        mat.color = current;
        mat.SetColor("_EmissionColor", current * t * 2f);
        mat.EnableKeyword("_EMISSION");
    }
}