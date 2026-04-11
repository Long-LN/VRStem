using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class SmarthomeTutorialManager : MonoBehaviour
{
    public static SmarthomeTutorialManager Instance;

    [System.Serializable]
    public class TutorialStage
    {
        public string stageName; // Ghi chú cho bạn dễ nhìn (VD: "1. Giới thiệu", "2. Bật Quạt")
        [TextArea(2, 5)] 
        public string[] dialogues; // Danh sách các câu nói trong giai đoạn này
        
        [Tooltip("Bật tick nếu yêu cầu người dùng PHẢI làm xong nhiệm vụ mới được đi tiếp")]
        public bool waitForAction; 
    }

    [Header("--- Cấu hình Kịch bản ---")]
    public List<TutorialStage> stages;

    [Header("--- Cấu hình UI ---")]
    public GameObject tutorialUI; 
    public TextMeshProUGUI dialogueText;
    public Button nextButton;

    private int currentStageIndex = 0;
    private int currentDialogueIndex = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        // Gắn sự kiện cho nút "Tiếp tục"
        if (nextButton != null) nextButton.onClick.AddListener(OnNextButtonClicked);
        
        // Bắt đầu kịch bản
        StartStage(0);
    }

    public void StartStage(int stageIndex)
    {
        if (stageIndex >= stages.Count)
        {
            // HẾT KỊCH BẢN -> Chuyển sang Free Roam
            tutorialUI.SetActive(false);
            return;
        }

        currentStageIndex = stageIndex;
        currentDialogueIndex = 0;
        tutorialUI.SetActive(true);
        UpdateDialogueUI();
    }

    public void OnNextButtonClicked()
    {
        currentDialogueIndex++;
        
        // Nếu đã đọc hết các câu chữ trong Giai đoạn hiện tại:
        if (currentDialogueIndex >= stages[currentStageIndex].dialogues.Length)
        {
            // Kiểm tra xem có đang bị kẹt lại bắt làm nhiệm vụ không?
            if (stages[currentStageIndex].waitForAction)
            {
                // Giấu nút Next đi, ép người dùng phải đi làm nhiệm vụ
                nextButton.gameObject.SetActive(false);
                currentDialogueIndex--; // Giữ nguyên dòng text cuối cùng (VD: "Hãy lấy cái quạt cắm vào trần nhà đi")
            }
            else
            {
                // Nếu không yêu cầu nhiệm vụ -> Chuyển thẳng sang Giai đoạn tiếp theo
                StartStage(currentStageIndex + 1);
            }
        }
        else
        {
            UpdateDialogueUI();
        }
    }

    private void UpdateDialogueUI()
    {
        if (dialogueText != null)
        {
            dialogueText.text = stages[currentStageIndex].dialogues[currentDialogueIndex];
        }

        // Hiện nút Next (trừ khi đang ở câu cuối và bị ép làm nhiệm vụ)
        bool isLastSentence = currentDialogueIndex == stages[currentStageIndex].dialogues.Length - 1;
        bool waitingForTask = stages[currentStageIndex].waitForAction;
        
        nextButton.gameObject.SetActive(!(isLastSentence && waitingForTask));
    }

    // CÁC HỆ THỐNG KHÁC SẼ GỌI HÀM NÀY KHI NGƯỜI DÙNG HOÀN THÀNH NHIỆM VỤ
    public void CompleteTask(string taskName)
    {
        // Chỉ cho phép qua bài nếu đúng là giai đoạn này đang yêu cầu làm nhiệm vụ
        if (stages[currentStageIndex].waitForAction && stages[currentStageIndex].stageName == taskName)
        {
            StartStage(currentStageIndex + 1);
        }
    }
}