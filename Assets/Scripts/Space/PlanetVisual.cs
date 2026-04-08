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
        if (marker != null) marker.SetActive(true);
        if (model != null)  model.SetActive(false);
    }

    public void ShowModel()
    {
        if (marker != null) marker.SetActive(false);
        if (model != null)
        {
            model.SetActive(true);
            model.transform.position = marker != null ? marker.transform.position : transform.position;
        }
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

    public void ShowInfo()
    {
        if (infoPanel == null) return;
        infoPanel.SetActive(true);

        if (descriptionText != null)
            descriptionText.text = description;
    }

    public void HideInfo()
    {
        if (infoPanel != null)
            infoPanel.SetActive(false);
    }
}