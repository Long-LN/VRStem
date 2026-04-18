using UnityEngine;
using TMPro;

public class VRReflectionUI : MonoBehaviour
{
    [Header("Text UI")]
    public TextMeshProUGUI angleInText;
    public TextMeshProUGUI angleOutText;

    [Header("Line Renderers")]
    public LineRenderer incomingLine;
    public LineRenderer normalLine;
    public LineRenderer reflectedLine;

    [Header("Arc Renderers")]
    public LineRenderer incomingArc;
    public LineRenderer reflectedArc;

    [Header("Hit Point")]
    public Transform hitPoint;

    [Header("Settings")]
    public float lineLength = 1.5f;
    public int arcSegments = 20;
    public float arcRadius = 0.5f;

    [Header("Colors (Bảng chọn màu)")]
    public Color incomingColor = Color.yellow;
    public Color normalColor = Color.red;
    public Color reflectedColor = Color.green;

    void Start()
    {
        ApplyColors();
    }

    void Update()
    {
        if (Camera.main != null)
        {
            Transform cam = Camera.main.transform;
            transform.LookAt(cam);
            transform.Rotate(0, 180, 0);
        }
    }

    void ApplyColors()
    {
        if (incomingLine != null) { incomingLine.startColor = incomingColor; incomingLine.endColor = incomingColor; }
        if (normalLine != null) { normalLine.startColor = normalColor; normalLine.endColor = normalColor; }
        if (reflectedLine != null) { reflectedLine.startColor = reflectedColor; reflectedLine.endColor = reflectedColor; }

        if (incomingArc != null) { incomingArc.startColor = incomingColor; incomingArc.endColor = incomingColor; }
        if (reflectedArc != null) { reflectedArc.startColor = reflectedColor; reflectedArc.endColor = reflectedColor; }

        if (angleInText != null) angleInText.color = incomingColor;
        if (angleOutText != null) angleOutText.color = reflectedColor;
    }

    public void UpdateAngles(Vector3 incoming, Vector3 normal, Vector3 reflected, Vector3 hitPos)
    {
        // ===== TEXT =====
        float angleIn = Vector3.Angle(-incoming, normal);
        float angleOut = Vector3.Angle(reflected, normal);

        angleInText.text = "Góc tới: " + angleIn.ToString("F1") + "°";
        angleOutText.text = "Góc phản xạ: " + angleOut.ToString("F1") + "°";

        // ===== HIT POINT =====
        if (hitPoint != null)
        {
            hitPoint.position = hitPos;
        }

        DrawLines(incoming, normal, reflected);

        DrawArcs(incoming, normal, reflected, hitPos);
    }

    void DrawLines(Vector3 incoming, Vector3 normal, Vector3 reflected)
    {
        if (hitPoint == null) return;

        Vector3 origin = hitPoint.position;

        // 🔴 Tia tới
        if (incomingLine != null)
        {
            incomingLine.positionCount = 2;
            incomingLine.SetPosition(0, origin);
            incomingLine.SetPosition(1, origin - incoming * lineLength);
        }

        // 🟡 Pháp tuyến
        if (normalLine != null)
        {
            normalLine.positionCount = 2;
            normalLine.SetPosition(0, origin);
            normalLine.SetPosition(1, origin + normal * lineLength);
        }

        // 🟢 Tia phản xạ
        if (reflectedLine != null)
        {
            reflectedLine.positionCount = 2;
            reflectedLine.SetPosition(0, origin);
            reflectedLine.SetPosition(1, origin + reflected * lineLength);
        }
    }

    void DrawArcs(Vector3 incoming, Vector3 normal, Vector3 reflected, Vector3 center)
    {
        // 🔴 Góc tới (normal → -incoming)
        DrawArc(incomingArc, normal, -incoming, center, Vector3.Cross(normal, incoming));

        // 🟢 Góc phản xạ (normal → reflected)
        DrawArc(reflectedArc, normal, reflected, center, Vector3.Cross(normal, reflected));
    }

    void DrawArc(LineRenderer arc, Vector3 from, Vector3 to, Vector3 center, Vector3 axis)
    {
        if (arc == null) return;

        arc.positionCount = arcSegments + 1;

        float angle = Vector3.SignedAngle(from, to, axis);

        for (int i = 0; i <= arcSegments; i++)
        {
            float t = i / (float)arcSegments;
            float currentAngle = Mathf.Lerp(0, angle, t);

            Vector3 dir = Quaternion.AngleAxis(currentAngle, axis) * from;
            arc.SetPosition(i, center + dir.normalized * arcRadius);
        }
    }

    public void ShowUI(bool value)
    {
        gameObject.SetActive(value);

        if (!value)
        {
            ClearAll();
        }
    }

    void ClearAll()
    {
        if (incomingLine != null) incomingLine.positionCount = 0;
        if (normalLine != null) normalLine.positionCount = 0;
        if (reflectedLine != null) reflectedLine.positionCount = 0;

        if (incomingArc != null) incomingArc.positionCount = 0;
        if (reflectedArc != null) reflectedArc.positionCount = 0;
    }
}