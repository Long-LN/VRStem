using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class TemperatureDisplay : MonoBehaviour
{
    [Header("Box Reference")]
    public Transform targetBox;
    public float maxSpeed = 5f;
    public float minCelsius = 0f;
    public float maxCelsius = 100f;

    [Header("World Space UI")]
    public TMP_Text worldTempText;
    public Image worldThermometerFill;
    public Image worldPanelBackground;

    [Header("Show Canvas Settings")]
    public GameObject worldTempCanvas;  // kéo WorldTempCanvas vào đây
    public int requiredMolecules = 15;  // số phân tử cần đủ

    List<Rigidbody> molecules = new();
    float celsius = 0f;
    float findTimer = 0f;
    bool canvasShown = false;

    void Start()
    {
        // Ẩn canvas lúc đầu
        if (worldTempCanvas != null)
            worldTempCanvas.SetActive(false);
    }

    void Update()
    {
        findTimer += Time.deltaTime;
        if (findTimer > 1f)
        {
            FindMolecules();
            findTimer = 0f;
        }

        // Kiểm tra đủ phân tử → hiện canvas
        if (!canvasShown && molecules.Count >= requiredMolecules)
        {
            canvasShown = true;
            if (worldTempCanvas != null)
                worldTempCanvas.SetActive(true);

            Debug.Log($"✅ Đủ {requiredMolecules} phân tử → hiện canvas nhiệt độ");
        }

        celsius = CalcCelsius();

        if (canvasShown)
            UpdateWorldUI();
    }

    void FindMolecules()
    {
        molecules.RemoveAll(rb => rb == null);

        foreach (var mf in FindObjectsByType<MoleculeFloat>(FindObjectsSortMode.None))
        {
            if (!mf.IsConfined()) continue;

            if (targetBox != null)
            {
                Vector3 local = targetBox.InverseTransformPoint(mf.transform.position);
                if (Mathf.Abs(local.x) > 0.6f ||
                    Mathf.Abs(local.y) > 0.6f ||
                    Mathf.Abs(local.z) > 0.6f) continue;
            }

            Rigidbody rb = mf.GetComponent<Rigidbody>();
            if (rb != null && !molecules.Contains(rb))
                molecules.Add(rb);
        }
    }

    float CalcCelsius()
    {
        molecules.RemoveAll(rb => rb == null);
        if (molecules.Count == 0) return 0f;

        float total = 0f;
        foreach (var rb in molecules)
            total += rb.linearVelocity.magnitude;

        float avg = total / molecules.Count;
        return Mathf.Lerp(minCelsius, maxCelsius, Mathf.Clamp01(avg / maxSpeed));
    }

    void UpdateWorldUI()
    {
        float t = Mathf.Clamp01(celsius / maxCelsius);
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
