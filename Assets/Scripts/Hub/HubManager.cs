using UnityEngine;

public class HubManager : MonoBehaviour
{
    [Header("Menu Panels")]
    public GameObject welcomePanel;
    public GameObject lessonPanel;

    [Header("AI Guide")]
    public HubAIGuide hubAI;

    // Hàm này sẽ gắn vào nút START
    public void OnStartButtonPressed()
    {
        Debug.Log("Người chơi đã bấm Start!");

        // 1. Chuyển đổi màn hình UI
        welcomePanel.SetActive(false);
        lessonPanel.SetActive(true);

        // 2. Ra lệnh cho AI nói phần tiếp theo
        if (hubAI != null)
        {
            hubAI.StartIntroPart2();
        }
    }
}