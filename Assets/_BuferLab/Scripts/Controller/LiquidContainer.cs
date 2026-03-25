using UnityEngine;
using UnityEngine.Events;

public class LiquidContainer : MonoBehaviour
{
    public LiquidData liquidData;
    public float maxVolume = 250f; 

    [Header("Ket noi Do hoa")]
    public Renderer liquidRenderer;
    public float shaderMinFill = -0.5f;
    public float shaderMaxFill = 0.5f;

    [Header("Che do mau Sac")]
    public bool showPHColorMode = false;
    
    [Header("Hieu ung phan ung")]
    [Tooltip("So cang nho mau chuyen cang cham (VD: 1 hoac 2)")]
    public float colorTransitionSpeed = 2f; 

    public UnityEvent OnLiquidChanged;

    private Color currentVisualColor;
    private Color targetColor;

    void Start()
    {
        if (liquidData == null) liquidData = new LiquidData();
        
        // Khoi tao mau ban dau ngay khi bat chay
        targetColor = showPHColorMode ? 
            ChemistryEngine.Instance.GetColorFromPH(liquidData.phValue) : 
            liquidData.liquidColor;
        currentVisualColor = targetColor;
        
        UpdateVisuals();
    }

    void Update()
    {
        if (liquidRenderer != null)
        {
            // Noi suy mau sac tu tu theo thoi gian o moi khung hinh
            currentVisualColor = Color.Lerp(currentVisualColor, targetColor, Time.deltaTime * colorTransitionSpeed);
            liquidRenderer.material.SetColor("_LiquidColor", currentVisualColor);
        }
    }

    public void ReceiveLiquid(LiquidData incomingData)
    {
        if (liquidData.volume + incomingData.volume > maxVolume)
        {
            float overflow = (liquidData.volume + incomingData.volume) - maxVolume;
            incomingData = incomingData.Extract(incomingData.volume - overflow);
        }
        
        if (ChemistryEngine.Instance != null)
        {
            ChemistryEngine.Instance.MixLiquids(liquidData, incomingData);
        }
        
        UpdateVisuals();
        OnLiquidChanged?.Invoke();
    }

    public void TogglePHMode()
    {
        showPHColorMode = !showPHColorMode;
        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        if (liquidRenderer != null)
        {
            float fillRatio = liquidData.volume / maxVolume;
            float currentFill = Mathf.Lerp(shaderMinFill, shaderMaxFill, fillRatio);
            liquidRenderer.material.SetFloat("_FillLevel", currentFill);

            // Chi tinh toan mau dich den (Target Color)
            // Mau thuc te hien thi se tu tu duoi theo Target Color trong ham Update
            targetColor = showPHColorMode ? 
                ChemistryEngine.Instance.GetColorFromPH(liquidData.phValue) : 
                liquidData.liquidColor;
        }
    }
}