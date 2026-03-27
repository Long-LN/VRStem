using UnityEngine;

public class Wobble : MonoBehaviour
{
    [Tooltip("Keo Mesh Renderer cua khoi nuoc vao day")]
    public Renderer liquidRenderer;

    public float MaxWobble = 0.03f;
    public float WobbleSpeed = 1f;
    public float Recovery = 1f;

    // DA SUA LOI O DAY: Doi tu Vector3 thanh float
    private float wobbleAmountToAddX;
    private float wobbleAmountToAddZ;
    private float wobbleAmountX;
    private float wobbleAmountZ;
    private float time;

    private Vector3 lastPos;
    private Vector3 lastRot;
    private Vector3 velocity;
    private Vector3 angularVelocity;

    void Start()
    {
        if (liquidRenderer == null) liquidRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
        time += Time.deltaTime;

        velocity = (lastPos - transform.position) / Time.deltaTime;
        angularVelocity = transform.rotation.eulerAngles - lastRot;

        wobbleAmountToAddX += Mathf.Clamp((velocity.x + angularVelocity.z * 0.2f) * MaxWobble, -MaxWobble, MaxWobble);
        wobbleAmountToAddZ += Mathf.Clamp((velocity.z + angularVelocity.x * 0.2f) * MaxWobble, -MaxWobble, MaxWobble);

        wobbleAmountX = wobbleAmountToAddX * Mathf.Sin(time * WobbleSpeed);
        wobbleAmountZ = wobbleAmountToAddZ * Mathf.Sin(time * WobbleSpeed);

        liquidRenderer.material.SetFloat("_WobbleX", wobbleAmountX);
        liquidRenderer.material.SetFloat("_WobbleZ", wobbleAmountZ);

        wobbleAmountToAddX = Mathf.Lerp(wobbleAmountToAddX, 0, Time.deltaTime * Recovery);
        wobbleAmountToAddZ = Mathf.Lerp(wobbleAmountToAddZ, 0, Time.deltaTime * Recovery);

        lastPos = transform.position;
        lastRot = transform.rotation.eulerAngles;
    }
}