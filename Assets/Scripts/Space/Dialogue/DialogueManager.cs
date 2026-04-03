using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    public AudioSource audioSource;
    public TextMeshProUGUI subtitleText;

    private void Awake() { Instance = this; }

    // Hàm gọi để bắt đầu chuỗi hội thoại
    public void PlaySequence(DialogueSequenceSO sequence)
    {
        if (sequence.playOnlyOnce && sequence.hasBeenPlayed) return;

        StopAllCoroutines();
        StartCoroutine(ExecuteSequence(sequence));

        if (sequence.playOnlyOnce) sequence.hasBeenPlayed = true;
    }

    private IEnumerator ExecuteSequence(DialogueSequenceSO sequence)
    {
        foreach (var segment in sequence.segments)
        {
            // 1. Hiển thị chữ và phát âm thanh
            subtitleText.text = $"{segment.subTitle}";
            audioSource.clip = segment.voiceClip;
            audioSource.Play();

            // 2. Đợi cho đến khi âm thanh phát xong
            yield return new WaitWhile(() => audioSource.isPlaying);

            // 3. Nghỉ thêm một khoảng thời gian (delayAfter)
            subtitleText.text = ""; // Xóa chữ trong lúc nghỉ
            yield return new WaitForSeconds(segment.delayAfter);
        }
    }
}