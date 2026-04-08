//using UnityEngine;
//using TMPro;
//using static GameFlowManager;

//public class TaskManager : MonoBehaviour
//{
//    [Header("Task State")]
//    public bool task1Done = false;
//    public bool task2Done = false;

//    [Header("Task 1 Setting")]
//    public float targetAngle = 30f;
//    public float tolerance = 10f;

//    [Header("UI")]
//    public GameObject taskUIRoot; // 🔥 thêm (canvas chứa task)
//    public TextMeshProUGUI task1Text;
//    public TextMeshProUGUI task2Text;

//    [Header("AI Guide")]
//    public AIGuide aiGuide;

//    [Header("Sound")]
//    public AudioSource audioSource;
//    public AudioClip successSound;

//    void Start()
//    {
//        // ❌ Ẩn toàn bộ task UI lúc đầu
//        if (taskUIRoot != null)
//            taskUIRoot.SetActive(false);

//        // reset text
//        if (task1Text != null)
//            task1Text.text = "☐ Task 1: Góc 30°";

//        if (task2Text != null)
//            task2Text.text = "☐ Task 2: Qua 2 gương";
//    }

//    public void EnableTask1()
//    {
//        if (task1Text != null)
//            task1Text.color = Color.white;
//    }

//    public void EnableTask2()
//    {
//        if (task2Text != null)
//            task2Text.color = Color.white;
//    }

//    // ================= BẮT ĐẦU BÀI HỌC =================
//    public void StartTasks()
//    {
//        if (taskUIRoot != null)
//            taskUIRoot.SetActive(true);
//    }

//    // ================= TASK 1 =================
//    public void CheckTask1(float angleIn, float angleOut)
//    {
//        if (task1Done) return;

//        Debug.Log($"AngleIn: {angleIn}, AngleOut: {angleOut}");

//        // ❗ CHỈ CHO PHÉP KHI ĐANG Ở TASK1
//        if (GameFlowManager.Instance.currentState != GameState.Task1)
//            return;

//        if (Mathf.Abs(angleIn - targetAngle) < tolerance &&
//            Mathf.Abs(angleOut - targetAngle) < tolerance)
//        {
//            task1Done = true;

//            Debug.Log("✅ Task 1 Done");

//            if (task1Text != null)
//            {
//                task1Text.text = "[✓] Task 1: Góc 30°";
//                task1Text.color = Color.green;
//            }

//            PlaySuccessSound();

//            // 👉 AI nói
//            if (aiGuide != null)
//                aiGuide.OnTask1Done();

//            // 👉 CHUYỂN SANG TASK 2
//            GameFlowManager.Instance.StartTask2();
//        }
//    }

//    // ================= TASK 2 =================
//    public void CheckTask2(int reflectionCount)
//    {
//        if (task2Done) return;

//        // ❗ CHỈ CHO PHÉP KHI ĐANG TASK 2
//        if (GameFlowManager.Instance.currentState != GameState.Task2)
//            return;

//        Debug.Log("Reflection Count: " + reflectionCount);

//        if (reflectionCount >= 2)
//        {
//            task2Done = true;

//            Debug.Log("✅ Task 2 Done");

//            if (task2Text != null)
//            {
//                task2Text.text = "[✓] Task 2: Qua 2 gương";
//                task2Text.color = Color.green;
//            }

//            PlaySuccessSound();

//            // 👉 AI nói
//            if (aiGuide != null)
//                aiGuide.OnTask2Done();

//            GameFlowManager.Instance.Finish();
//        }
//    }

//    // ================= SOUND =================
//    void PlaySuccessSound()
//    {
//        if (audioSource != null && successSound != null)
//        {
//            audioSource.PlayOneShot(successSound);
//        }
//    }
//}

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static GameFlowManager;

public class TaskManager : MonoBehaviour
{
    [Header("Task State")]
    public bool task1Done = false;
    public bool task2Done = false;

    [Header("Task 1 Setting")]
    public float targetAngle = 30f;
    public float tolerance = 10f;

    [Header("UI ROOT")]
    public GameObject taskUIRoot;

    [Header("Task 1 UI")]
    public TextMeshProUGUI task1Text;
    public Image task1Icon;

    [Header("Task 2 UI")]
    public TextMeshProUGUI task2Text;
    public Image task2Icon;

    [Header("Icon Sprites")]
    public Sprite uncheckedSprite;
    public Sprite checkedSprite;

    [Header("AI Guide")]
    public AIGuide aiGuide;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip successSound;

    void Start()
    {
        // Ẩn UI lúc đầu
        if (taskUIRoot != null)
            taskUIRoot.SetActive(false);

        // Reset Task 1
        if (task1Text != null)
            task1Text.text = "Task 1: Góc 30°";

        if (task1Icon != null)
            task1Icon.sprite = uncheckedSprite;

        // Reset Task 2
        if (task2Text != null)
            task2Text.text = "Task 2: Qua 2 gương";

        if (task2Icon != null)
            task2Icon.sprite = uncheckedSprite;
    }

    public void EnableTask1()
    {
        if (task1Text != null)
            task1Text.color = Color.white;
    }

    public void EnableTask2()
    {
        if (task2Text != null)
            task2Text.color = Color.white;
    }

    // ================= START =================
    public void StartTasks()
    {
        if (taskUIRoot != null)
            taskUIRoot.SetActive(true);
    }

    // ================= TASK 1 =================
    public void CheckTask1(float angleIn, float angleOut)
    {
        if (task1Done) return;

        if (GameFlowManager.Instance.currentState != GameState.Task1)
            return;

        if (Mathf.Abs(angleIn - targetAngle) < tolerance &&
            Mathf.Abs(angleOut - targetAngle) < tolerance)
        {
            task1Done = true;

            Debug.Log("✅ Task 1 Done");

            // 🎯 UPDATE UI
            if (task1Icon != null)
                task1Icon.sprite = checkedSprite;

            if (task1Text != null)
            {
                task1Text.color = Color.green;
                task1Text.text = "Task 1: Góc 30°";
            }

            PlaySuccessSound();

            if (aiGuide != null)
                aiGuide.OnTask1Done();

            GameFlowManager.Instance.StartTask2();
        }
    }

    // ================= TASK 2 =================
    public void CheckTask2(int reflectionCount)
    {
        if (task2Done) return;

        if (GameFlowManager.Instance.currentState != GameState.Task2)
            return;

        if (reflectionCount >= 2)
        {
            task2Done = true;

            Debug.Log("✅ Task 2 Done");

            // 🎯 UPDATE UI
            if (task2Icon != null)
                task2Icon.sprite = checkedSprite;

            if (task2Text != null)
            {
                task2Text.color = Color.green;
                task2Text.text = "Task 2: Qua 2 gương";
            }

            PlaySuccessSound();

            if (aiGuide != null)
                aiGuide.OnTask2Done();

            GameFlowManager.Instance.Finish();
        }
    }

    // ================= SOUND =================
    void PlaySuccessSound()
    {
        if (audioSource != null && successSound != null)
        {
            audioSource.PlayOneShot(successSound);
        }
    }
}