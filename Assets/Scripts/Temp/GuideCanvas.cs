using UnityEngine;
using UnityEngine.UI;

public class GuideCanvas : MonoBehaviour
{
    public GameObject guidePanel; // kéo GuideCanvas vào

    void Start()
    {
        // Hiện canvas lúc bắt đầu
        if (guidePanel != null)
            guidePanel.SetActive(true);
    }

    // Gán vào Button OnClick
    public void CloseGuide()
    {
        if (guidePanel != null)
            guidePanel.SetActive(false);
    }
}