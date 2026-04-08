using System;
using UnityEngine;
using TMPro;

public class PlanetVisual : MonoBehaviour
{
    [Header("Model")]
    public GameObject marker;
    public GameObject model;

    [Header("Tooltip")]
    public string planetName = "";
    public GameObject tooltipPanel;
    public TextMeshProUGUI tooltipText;

    [Header("Label trên đầu hành tinh nhỏ")]
    public GameObject labelPanel;
    public TextMeshProUGUI labelText;

    [Header("Bảng thông tin")]
    [TextArea] public string description = "";
    public GameObject infoPanel;
    public TextMeshProUGUI descriptionText;
    public Canvas infoCanvas;

    public bool isAnswered;

    private void Awake()
    {
        if (planetName == "")
            planetName = gameObject.name;
    }

    private void Start()
    {
        if (infoPanel != null) infoPanel.SetActive(false);
    }

    public void ShowMarker()
    {
        Debug.Log(gameObject.name + " show marker");
        marker.SetActive(true);
        model.SetActive(false);
    }

    public void ShowModel()
    {
        Debug.Log(gameObject.name + " show model");
        marker.SetActive(false);
        model.SetActive(true);
        model.transform.position = marker.transform.position;
    }

    public void ShowTooltip()
    {
        if (tooltipPanel == null) return;
        tooltipPanel.SetActive(true);
        if (tooltipText != null)
            tooltipText.text = string.IsNullOrEmpty(planetName) ? "?" : planetName;
    }

    public void HideTooltip()
    {
        if (tooltipPanel != null)
            tooltipPanel.SetActive(false);
    }

    public void ShowQuestionMark()
    {
        if (labelPanel == null) return;
        labelPanel.SetActive(true);
        if (labelText != null)
        {
            labelText.text = "?";
            labelText.color = Color.white;
        }
    }

    public void ShowCorrectLabel()
    {
        if (labelPanel == null) return;
        labelPanel.SetActive(true);
        if (labelText != null)
        {
            labelText.text = planetName;
            labelText.color = Color.green;
        }
    }

    public void HideLabel()
    {
        if (labelPanel != null)
            labelPanel.SetActive(false);
    }

    public void ShowInfo(Camera cam, Vector3 planetWorldPos, float rightOffset = 0.8f, float upOffset = 0.5f)
    {
        if (infoPanel == null) return;
        infoPanel.SetActive(true);
        if (descriptionText != null)
            descriptionText.text = description;

        if (cam == null) return;

        // Lấy Canvas từ infoPanel hoặc cha của nó
        Canvas canvas = infoCanvas;
        if (canvas == null)
            canvas = infoPanel.GetComponent<Canvas>();
        if (canvas == null)
            canvas = infoPanel.GetComponentInParent<Canvas>();
        if (canvas == null) return;

        // Ép sang World Space nếu chưa phải
        if (canvas.renderMode != RenderMode.WorldSpace)
        {
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = cam;
            canvas.transform.localScale = Vector3.one * 0.001f;
        }

        // Đặt vị trí bên PHẢI hành tinh theo góc nhìn camera
        canvas.transform.position = planetWorldPos
            + cam.transform.right * rightOffset
            + cam.transform.up    * upOffset;

        // Quay mặt về phía camera
        Vector3 dir = cam.transform.position - canvas.transform.position;
        canvas.transform.rotation = Quaternion.LookRotation(-dir.normalized);
    }

    public void HideInfo()
    {
        if (infoPanel != null)
            infoPanel.SetActive(false);
    }
}