using UnityEngine;

public class AudioManagerTemp : MonoBehaviour
{
    public AudioSource audioSource;

    [Header("Panels")]
    public GameObject panel1; // vào phòng
    public GameObject panel2; // sau ví dụ 1
    public GameObject panel3; // sau khi bấm tiếp tục
    public GameObject panel4; // sau ví dụ 2

    [Header("Audio")]
    public AudioClip audio1;
    public AudioClip audio2;
    public AudioClip audio3;
    public AudioClip audio4;

    // ===== AUTO CHẠY KHI PLAY (TEST) =====
    void Start()
    {
        StartTutorial();
    }

    // ===== BẮT ĐẦU =====
    public void StartTutorial()
    {
        Show(panel1, audio1);
    }

    // ===== HOÀN THÀNH VÍ DỤ 1 =====
    public void CompletePart1()
    {
        Show(panel2, audio2);
    }

    // ===== NÚT TIẾP TỤC =====
    public void OnClickNext()
    {
        Show(panel3, audio3);
    }

    // ===== HOÀN THÀNH VÍ DỤ 2 =====
    public void CompletePart2()
    {
        Show(panel4, audio4);
    }

    // ===== HÀM CHUNG =====
    void Show(GameObject panel, AudioClip clip)
    {
        HideAllPanels();

        if (panel != null)
            panel.SetActive(true);

        if (audioSource != null && clip != null)
        {
            audioSource.Stop();
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    void HideAllPanels()
    {
        if (panel1 != null) panel1.SetActive(false);
        if (panel2 != null) panel2.SetActive(false);
        if (panel3 != null) panel3.SetActive(false);
        if (panel4 != null) panel4.SetActive(false);
    }
}