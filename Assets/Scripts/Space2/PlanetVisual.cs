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
    [TextArea] public string description = "";  // Điền mô tả hành tinh
    public GameObject infoPanel;                // Kéo InfoPanel vào đây
    public TextMeshProUGUI descriptionText;     // Kéo DescriptionText vào đây

    public void ShowMarker()
    {
        Debug.Log(gameObject.name + "show marker");
        marker.SetActive(true);
        model.SetActive(false);
    }

    public void ShowModel()
    {
        Debug.Log(gameObject.name + "show model");
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

    // Hiện bảng thông tin
    public void ShowInfo()
    {
        if (infoPanel == null) return;
        infoPanel.SetActive(true);
        if (descriptionText != null)
            descriptionText.text = description;
    }

    // Ẩn bảng thông tin
    public void HideInfo()
    {
        if (infoPanel != null)
            infoPanel.SetActive(false);
    }
    private void Start()
{
    // Ẩn InfoPanel lúc đầu
    if (infoPanel != null) infoPanel.SetActive(false);
}
}