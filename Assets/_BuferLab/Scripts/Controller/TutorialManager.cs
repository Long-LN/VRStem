using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    [Header("UI Kết nối")]
    public TMP_Text guideText; // Kéo TMP Text của bảng hướng dẫn vào đây
    public AudioSource voiceAudioSource; // AudioSource để phát giọng nói AI

    [Header("Dữ liệu bài học")]
    public string[] instructions; // Nhập các câu text ở trên vào mảng này trong Inspector
    public AudioClip[] voiceClips; // Kéo các file âm thanh tương ứng vào đây

    private int currentStep = 0;

    void Start()
    {
        ShowStep(0);
    }

    // Hàm này bạn sẽ gọi từ các sự kiện (ví dụ: nút "Next" trên bảng Guide)
    public void NextStep()
    {
        if (currentStep < instructions.Length - 1)
        {
            currentStep++;
            ShowStep(currentStep);
        }
    }

    public void PreviousStep()
    {
        if (currentStep > 0)
        {
            currentStep--;
            ShowStep(currentStep);
        }
    }

    private void ShowStep(int stepIndex)
    {
        // Hiển thị chữ lên bảng
        if (guideText != null) guideText.text = instructions[stepIndex];

        // Phát âm thanh hướng dẫn
        if (voiceAudioSource != null && voiceClips[stepIndex] != null)
        {
            voiceAudioSource.Stop();
            voiceAudioSource.clip = voiceClips[stepIndex];
            voiceAudioSource.Play();
        }
    }
}
