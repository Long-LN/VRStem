using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager Instance;

    [Header("State")]
    public GameState currentState = GameState.Idle;

    private bool hasStarted = false;

    [Header("References")]
    public AIGuide aiGuide;
    public GameObject taskUI;
    public TaskManager taskManager;

    public enum GameState
    {
        Idle,
        Intro,
        Task1,
        Task2,
        Finished
    }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // ❌ Tắt ban đầu
        if (aiGuide != null)
            aiGuide.canvasGroup.alpha = 0;

        if (taskUI != null)
            taskUI.SetActive(false);
    }

    // ================= START LESSON =================
    public void StartLesson()
    {
        if (hasStarted) return;

        hasStarted = true;

        Debug.Log("🚀 Start Lesson");

        currentState = GameState.Intro;

        // bắt đầu intro
        if (aiGuide != null)
            aiGuide.StartIntro();
    }

    // ================= TASK 1 =================
    public void StartTask1()
    {
        Debug.Log("🎯 Start Task 1");

        currentState = GameState.Task1;

        if (taskManager != null)
            taskManager.EnableTask1();
    }

    // ================= TASK 2 =================
    public void StartTask2()
    {
        Debug.Log("🎯 Start Task 2");

        currentState = GameState.Task2;

        if (taskManager != null)
            taskManager.EnableTask2();
    }

    // ================= FINISH =================
    public void Finish()
    {
        Debug.Log("🏁 Finished Lesson");

        currentState = GameState.Finished;
    }
}