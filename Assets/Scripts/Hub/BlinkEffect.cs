using UnityEngine;
using UnityEngine.UI;

public class BlinkEffect : MonoBehaviour
{
    [Header("UI Component")]
    public Image buttonBackground;

    [Header("Blink Settings")]
    public float blinkSpeed = 2f;      // Tốc độ nhấp nháy (số càng to càng nhanh)
    public float minAlpha = 0.3f;      // Độ mờ tối đa (0.3 là mờ đi 70%)
    public float maxAlpha = 1.0f;      // Độ rõ tối đa (1.0 là rõ hoàn toàn)

    void Update()
    {
        if (buttonBackground != null)
        {
            // Lấy màu hiện tại của nền
            Color currentColor = buttonBackground.color;

            // Dùng hàm PingPong để tính toán độ mờ nhấp nháy theo thời gian
            float currentAlpha = minAlpha + Mathf.PingPong(Time.time * blinkSpeed, maxAlpha - minAlpha);

            // Áp dụng độ mờ mới vào nền
            currentColor.a = currentAlpha;
            buttonBackground.color = currentColor;
        }
    }
}