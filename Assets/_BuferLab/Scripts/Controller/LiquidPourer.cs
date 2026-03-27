using UnityEngine;

[RequireComponent(typeof(LiquidContainer))]
public class LiquidPourer : MonoBehaviour
{
    [Header("Cau Hinh Vanh Coc")]
    public Transform rimCenter;
    public float rimRadius = 0.05f;

    [Header("Cau Hinh Rot Nuoc")]
    public float maxPourRate = 50f;
    public LiquidStream streamVisuals;
    
    [Header("Thong so Tinh toan")]
    public Transform cupBottom;
    public float pourSensitivityOffset = -0.01f; 

    [Header("Am Thanh")]
    [Tooltip("Keo component Audio Source cua cai coc vao day")]
    public AudioSource pourAudioSource;

    private LiquidContainer myContainer;
    private bool isPouring = false;

    void Start()
    {
        myContainer = GetComponent<LiquidContainer>();
        if (cupBottom == null) cupBottom = transform;
    }

    void Update()
    {
        if (myContainer.liquidData.volume <= 0f)
        {
            if (isPouring) StopPouring();
            return;
        }

        Vector3 lowestRimPoint = rimCenter.position;
        float minHeight = float.MaxValue;
        
        for (int i = 0; i < 36; i++)
        {
            float angle = i * 10f * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * rimRadius;
            Vector3 worldPoint = rimCenter.TransformPoint(offset);
            
            if (worldPoint.y < minHeight)
            {
                minHeight = worldPoint.y;
                lowestRimPoint = worldPoint;
            }
        }

        float fillRatio = Mathf.Clamp01(myContainer.liquidData.volume / myContainer.maxVolume);
        float currentWaterLevelY = Mathf.Lerp(cupBottom.position.y, rimCenter.position.y, fillRatio) + pourSensitivityOffset;

        if (currentWaterLevelY > lowestRimPoint.y)
        {
            float overflowAmount = currentWaterLevelY - lowestRimPoint.y;
            float tiltPercentage = Mathf.Clamp01(overflowAmount / (rimRadius * 2f));
            PourLiquid(tiltPercentage, lowestRimPoint);
        }
        else if (isPouring)
        {
            StopPouring();
        }
    }

    void PourLiquid(float tiltPercentage, Vector3 pourPosition)
    {
        isPouring = true;
        float currentPourRate = maxPourRate * tiltPercentage;
        float amountToPour = currentPourRate * Time.deltaTime;

        if (myContainer.liquidData.volume < amountToPour) amountToPour = myContainer.liquidData.volume;

        LiquidData pouredLiquid = myContainer.liquidData.Extract(amountToPour);
        myContainer.UpdateVisuals();

        RaycastHit hit;
        Vector3 targetPoint = pourPosition + Vector3.down * 1.5f;

        if (Physics.Raycast(pourPosition, Vector3.down, out hit, 1.5f))
        {
            targetPoint = hit.point;
            LiquidContainer targetContainer = hit.collider.GetComponentInParent<LiquidContainer>();
            if (targetContainer != null && targetContainer != myContainer)
            {
                targetContainer.ReceiveLiquid(pouredLiquid);
            }
        }

        if (streamVisuals != null)
        {
            float streamWidth = Mathf.Lerp(0.005f, 0.02f, tiltPercentage);
            Color streamColor = myContainer.showPHColorMode ? 
                ChemistryEngine.Instance.GetColorFromPH(pouredLiquid.phValue) : 
                pouredLiquid.liquidColor;
            
            streamVisuals.BeginPour(streamColor, streamWidth);
            Vector3 pourDirection = (pourPosition - rimCenter.position).normalized;
            Vector3 initialVelocity = pourDirection * (1.5f * tiltPercentage); 
            streamVisuals.UpdateParabola(pourPosition, targetPoint, initialVelocity);
        }

        // --- XU LY AM THANH ROT NUOC ---
        if (pourAudioSource != null)
        {
            if (!pourAudioSource.isPlaying)
            {
                pourAudioSource.Play();
            }
            
            // 1. Dieu chinh AM LUONG (Volume)
            // tiltPercentage (0 den 1) dai dien cho do nghieng va luong nuoc dang chay
            // Khi rot ri ri, am luong chi o muc 5% (0.05f). Khi doc nguoc coc, am luong len 100% (1.0f)
            pourAudioSource.volume = Mathf.Lerp(0.05f, 1.0f, tiltPercentage);

            // 2. Dieu chinh DO THANH/TRAM (Pitch) de tang tinh chan thuc
            // Khi rot ri ri (nuoc it), tieng se thanh hon, roc rach hon (Pitch = 1.2)
            // Khi rot manh (nuoc nhieu), tieng se duc va on a hon (Pitch = 0.8)
            pourAudioSource.pitch = Mathf.Lerp(1.2f, 0.8f, tiltPercentage);
        }
    }

    void StopPouring()
    {
        isPouring = false;
        if (streamVisuals != null) streamVisuals.EndPour();

        // --- TAT AM THANH KHI DUNG ROT ---
        if (pourAudioSource != null && pourAudioSource.isPlaying)
        {
            pourAudioSource.Stop();
        }
    }
}