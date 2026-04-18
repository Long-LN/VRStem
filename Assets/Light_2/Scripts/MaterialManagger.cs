using UnityEngine;

public class IsolatedGlowManager : MonoBehaviour
{
    [Header("1. Core Object (Vật thể chính)")]
    public MeshRenderer targetMesh;

    [ColorUsage(true, true)]
    public Color glowColor = Color.cyan;
    public float objectIntensity = 2.5f;

    [Header("2. Fake Bloom (Quầng sáng giả lập)")]
    [Tooltip("Dùng Material của tia sáng (URP/Unlit, Transparent Additive)")]
    public Material haloMaterial;
    public float haloSize = 1.3f;

    [Range(0, 1)]
    public float haloAlpha = 0.4f;

    [Header("3. Animation (Hiệu ứng nhịp thở)")]
    public bool isPulse = false;
    public float pulseSpeed = 2f;
    public float pulseMin = 0.7f;
    public float pulseMax = 1.3f;

    private LineRenderer haloRenderer;
    private Material instancedMeshMat;
    private static readonly int EmissionColorProp = Shader.PropertyToID("_EmissionColor");

    void Start()
    {
        // Khởi tạo Material riêng để không ảnh hưởng đến các vật thể khác dùng chung Material
        if (targetMesh != null)
        {
            instancedMeshMat = targetMesh.material;
            instancedMeshMat.EnableKeyword("_EMISSION");
        }

        // Tự động tạo Halo quanh vật thể
        CreateHalo();
    }

    void CreateHalo()
    {
        GameObject haloObj = new GameObject(gameObject.name + "_Halo_Renderer");
        haloObj.transform.SetParent(transform);
        haloObj.transform.localPosition = Vector3.zero;

        haloRenderer = haloObj.AddComponent<LineRenderer>();
        haloRenderer.material = haloMaterial;
        haloRenderer.useWorldSpace = false;
        haloRenderer.loop = true;
        haloRenderer.numCapVertices = 12; // Làm vòng halo mịn hơn

        // Vẽ vòng tròn giả lập hiệu ứng lóa (Bloom)
        int segments = 32;
        haloRenderer.positionCount = segments;
        for (int i = 0; i < segments; i++)
        {
            float angle = i * Mathf.PI * 2 / segments;
            haloRenderer.SetPosition(i, new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0));
        }
    }

    void Update()
    {
        float currentIntensity = objectIntensity;

        // Xử lý hiệu ứng nhấp nháy nếu bật
        if (isPulse)
        {
            float lerp = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;
            currentIntensity *= Mathf.Lerp(pulseMin, pulseMax, lerp);
        }

        // 1. Cập nhật độ sáng lõi (Emission)
        if (instancedMeshMat != null)
        {
            instancedMeshMat.SetColor(EmissionColorProp, glowColor * currentIntensity);
        }

        // 2. Cập nhật Quầng sáng (Halo)
        if (haloRenderer != null)
        {
            // Màu quầng sáng đi theo màu gốc nhưng có độ trong suốt (Alpha)
            Color finalHaloColor = glowColor;
            finalHaloColor.a = haloAlpha;

            haloRenderer.startColor = haloRenderer.endColor = finalHaloColor;

            // QUAN TRỌNG: Luôn quay mặt về phía Camera để nhìn giống hiệu ứng Bloom thật
            if (Camera.main != null)
            {
                haloRenderer.transform.LookAt(Camera.main.transform);
            }

            // Cập nhật kích thước quầng sáng
            haloRenderer.transform.localScale = Vector3.one * haloSize;

            // Độ dày của nét vẽ quầng sáng (tỉ lệ với kích thước halo)
            float width = haloSize * 0.25f;
            haloRenderer.startWidth = haloRenderer.endWidth = width;
        }
    }
}
