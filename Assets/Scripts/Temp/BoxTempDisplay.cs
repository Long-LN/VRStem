using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoxTempDisplay : MonoBehaviour
{
    [Header("Tham chiếu hộp")]
    public Transform targetBox;     // kéo GlassBox_Group vào
    public float boxHalfSize = 0.5f; // chỉnh cho vừa với hộp
    public float maxSpeed = 5f;
    public float minCelsius = 0f;
    public float maxCelsius = 100f;

    [Header("UI")]
    public TMP_Text tempText;
    public Image thermometerFill;
    public Image panelBackground;

    [Header("Canvas")]
    public GameObject displayCanvas;
    public int requiredMolecules = 15;

    float celsius = 0f;
    bool canvasShown = false;

    void Start()
    {
        if (displayCanvas != null)
            displayCanvas.SetActive(false);
    }

    void Update()
    {
        if (targetBox == null) return;

        int count = 0;
        float totalSpeed = 0f;

        foreach (var go in GameObject.FindGameObjectsWithTag("Molecule"))
        {
            Rigidbody rb = go.GetComponent<Rigidbody>();
            if (rb == null) continue;
            if (rb.isKinematic) continue;

            // Check khoảng cách đến tâm hộp
            float dist = Vector3.Distance(go.transform.position, targetBox.position);
            if (dist > boxHalfSize * 2f) continue;

            totalSpeed += rb.linearVelocity.magnitude;
            count++;
        }

        // Hiện canvas khi đủ phân tử
        if (!canvasShown && count >= requiredMolecules)
        {
            canvasShown = true;
            if (displayCanvas != null)
                displayCanvas.SetActive(true);
            Debug.Log($"✅ Đủ {count} phân tử → hiện canvas");
        }

        // Luôn cập nhật nhiệt độ
        float avgSpeed = count > 0 ? totalSpeed / count : 0f;
        celsius = Mathf.Lerp(minCelsius, maxCelsius,
                  Mathf.Clamp01(avgSpeed / maxSpeed));

        if (canvasShown)
            UpdateUI();
    }

    void UpdateUI()
    {
        float t = Mathf.Clamp01(celsius / maxCelsius);
        Color cold = new Color(0.2f, 0.5f, 1f, 0.85f);
        Color hot  = new Color(1f, 0.2f, 0.1f, 0.85f);
        Color cur  = Color.Lerp(cold, hot, t);

        if (tempText != null)
            tempText.text = $"{celsius:F1}°C";

        if (thermometerFill != null)
        {
            thermometerFill.fillAmount = t;
            thermometerFill.color = cur;
        }

        if (panelBackground != null)
            panelBackground.color = Color.Lerp(
                new Color(0.1f, 0.1f, 0.3f, 0.8f),
                new Color(0.3f, 0.05f, 0.05f, 0.8f), t);
    }
}