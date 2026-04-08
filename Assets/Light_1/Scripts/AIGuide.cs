using UnityEngine;
using TMPro;
using System.Collections;

public class AIGuide : MonoBehaviour
{
    public TextMeshProUGUI textUI;
    public CanvasGroup canvasGroup;
    public AudioSource audioSource;

    public float typingSpeed = 0.03f;

    private Coroutine currentCoroutine;

    // ================= AUDIO (8 FILE) =================
    [Header("Intro Voice")]
    public AudioClip intro1;
    public AudioClip intro2;
    public AudioClip intro3;
    public AudioClip intro4;
    public AudioClip intro5;

    [Header("Task Voice")]
    public AudioClip task1Voice;
    public AudioClip task2Voice;
    public AudioClip finishVoice;

    void Start()
    {
        // ❌ Không chạy khi vào game
        canvasGroup.alpha = 0;
    }

    // ================= INTRO =================
    public void StartIntro()
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(IntroFlow());
    }

    IEnumerator IntroFlow()
    {
        yield return SpawnEffect();

        yield return ShowMessage("Chào bạn, mình là trợ giảng của bài học này", intro1);

        yield return ShowMessage("Sau đây bạn cần làm 2 thử thách nhỏ để hoàn thành bài học!", intro2);

        yield return ShowMessage("1. Đầu tiên, bạn hãy soi đèn để tạo được góc tới = góc phản xạ = 30°", intro3);

        yield return ShowMessage("2. Bạn hãy thực hiện chiếu đèn phản xạ qua 2 gương liên tiếp", intro4);

        yield return ShowMessage("OK, chúng ta bắt đầu nhé!", intro5);

        // 🔥 HIỆN TASK UI
        if (GameFlowManager.Instance.taskManager != null)
        {
            GameFlowManager.Instance.taskManager.StartTasks();
        }

        // 👉 MỞ TASK 1
        GameFlowManager.Instance.StartTask1();
    }

    // ================= HIỂN THỊ TEXT + AUDIO =================
    public IEnumerator ShowMessage(string msg, AudioClip voice = null)
    {
        yield return FadeIn();

        textUI.text = "";

        // 🔊 phát audio
        if (voice != null && audioSource != null)
        {
            audioSource.clip = voice;
            audioSource.Play();
        }

        // ✨ typing effect
        foreach (char c in msg)
        {
            textUI.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        // ⏳ chờ audio xong
        if (voice != null && audioSource != null)
        {
            yield return new WaitWhile(() => audioSource.isPlaying);
        }
        else
        {
            yield return new WaitForSeconds(2f);
        }

        yield return FadeOut();
    }

    // ================= TASK EVENTS =================
    public void OnTask1Done()
    {
        ShowText("Bạn đã hoàn thành thử thách 1, hãy tiếp tục nhé!", task1Voice);
    }

    public void OnTask2Done()
    {
        ShowText("Bạn đã hoàn thành thử thách 2", task2Voice);
        StartCoroutine(EndMessage());
    }

    IEnumerator EndMessage()
    {
        yield return new WaitForSeconds(2f);
        ShowText("Chúc mừng bạn đã vượt qua 2 thử thách và hoàn thành bài học này!", finishVoice);
    }

    // ================= HELPER =================
    public void ShowText(string msg, AudioClip voice = null)
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(ShowMessage(msg, voice));
    }

    IEnumerator FadeIn()
    {
        canvasGroup.alpha = 0;

        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime * 2;
            yield return null;
        }
    }

    IEnumerator FadeOut()
    {
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime * 2;
            yield return null;
        }
    }

    IEnumerator SpawnEffect()
    {
        transform.localScale = Vector3.zero;

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 2;
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
            yield return null;
        }
    }

    // ================= VR POSITION =================
    void Update()
    {
        if (Camera.main != null)
        {
            Transform cam = Camera.main.transform;

            // 👉 luôn trước mặt người chơi
            transform.position = cam.position + cam.forward * 2.4f;

            transform.LookAt(cam);
            transform.Rotate(0, 180, 0);
        }
    }
}