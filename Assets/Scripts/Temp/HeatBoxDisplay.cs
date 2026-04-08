using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HeatBoxDisplay : MonoBehaviour
{
    [Header("Box Reference")]
    public HeatBox targetHeatBox;
    public float maxSpeed = 5f;
    public float minCelsius = 0f;
    public float maxCelsius = 100f;

    [Header("World Space UI")]
    public TMP_Text worldTempText;
    public Image worldThermometerFill;
    public Image worldPanelBackground;

    void Update()
    {
        if (targetHeatBox == null) return;

        float speed = targetHeatBox.GetTemperature();
        float t = Mathf.Clamp01(speed / maxSpeed);
        float celsius = Mathf.Lerp(minCelsius, maxCelsius, t);

        UpdateUI(celsius, t);
    }

    void UpdateUI(float celsius, float t)
    {
        Color cold = new Color(0.2f, 0.5f, 1f, 0.85f);
        Color hot  = new Color(1f, 0.2f, 0.1f, 0.85f);
        Color cur  = Color.Lerp(cold, hot, t);

        if (worldTempText != null)
            worldTempText.text = $"{celsius:F1}°C";

        if (worldThermometerFill != null)
        {
            worldThermometerFill.fillAmount = t;
            worldThermometerFill.color = cur;
        }

        if (worldPanelBackground != null)
            worldPanelBackground.color = Color.Lerp(
                new Color(0.1f, 0.1f, 0.3f, 0.8f),
                new Color(0.3f, 0.05f, 0.05f, 0.8f), t);
    }
}