using UnityEngine;

public class VRLiquidVolume : MonoBehaviour
{
    [Tooltip("Keo object LiquidMesh (khoi nuoc) vao o nay")]
    public MeshRenderer liquidRenderer;

    [Tooltip("Muc nuoc toi da va toi thieu cua binh chua")]
    public float maxFill = 0.5f;
    public float minFill = -0.5f;

    [Tooltip("Toc do nuoc chay ra khi up binh")]
    public float pourSpeed = 0.2f;

    [Tooltip("Muc nuoc hien tai")]
    [Range(-0.5f, 0.5f)]
    public float currentFillLevel = 0.2f;

    private Material liquidMaterial;
    private int fillLevelPropID;

    void Start()
    {
        if (liquidRenderer != null)
        {
            // Tao mot ban sao material de cac binh khong bi anh huong lan nhau
            liquidMaterial = liquidRenderer.material;

            // Luu ID bien cua Shader de toi uu hieu nang cho Quest 3
            fillLevelPropID = Shader.PropertyToID("_FillLevel");
        }
    }

    void Update()
    {
        if (liquidMaterial == null) return;

        // Tinh toan goc nghieng cua cai binh so voi truc thang dung cua the gioi
        float tiltAngle = Vector3.Angle(Vector3.up, transform.forward);

        // Neu binh bi doc nguoc qua 90 do, bat dau giam muc nuoc
        if (tiltAngle > 90f)
        {
            currentFillLevel -= pourSpeed * Time.deltaTime;

            // Dam bao nuoc khong bi rut qua muc toi thieu
            currentFillLevel = Mathf.Max(currentFillLevel, minFill);
        }

        // Cap nhat muc nuoc moi xuong Shader
        liquidMaterial.SetFloat(fillLevelPropID, currentFillLevel);
    }
}