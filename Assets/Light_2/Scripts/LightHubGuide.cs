using UnityEngine;
using System.Collections;

public class LightHubGuide : MonoBehaviour
{
    [Header("Âm thanh (Kéo 6 file MP3 vào đây)")]
    public AudioSource audioSource;
    public AudioClip intro1_Welcome;
    public AudioClip intro2_Science;
    public AudioClip intro3_Tech;
    public AudioClip intro4_Engineering;
    public AudioClip intro5_Math;
    public AudioClip intro6_Ending;

    [Header("Giao diện Text (Kéo 4 cục Text S-T-E-M vào đây)")]
    [Tooltip("Theo thứ tự: Khoa học, Công nghệ, Kỹ thuật, Toán học")]
    public GameObject[] stemTexts;

    private bool hasPlayed = false; // Biến đánh dấu để AI không nói lặp lại khi người chơi đi ra đi vào

    void Start()
    {
        // Ẩn tất cả các chữ S-T-E-M ngay từ đầu lúc mới vào Hub
        foreach (var txt in stemTexts)
        {
            if (txt != null) txt.SetActive(false);
        }
    }

    // Hàm này tự động kích hoạt khi người chơi bước vào vùng cảm biến
    private void OnTriggerEnter(Collider other)
    {
        // Kiểm tra xem đối tượng bước vào có phải là người chơi không, và AI đã nói chưa
        if (!hasPlayed && other.CompareTag("Player"))
        {
            hasPlayed = true; // Đánh dấu là đang nói rồi, không kích hoạt lại nữa
            StartCoroutine(PlayGuideSequence());
        }
    }

    IEnumerator PlayGuideSequence()
    {
        yield return StartCoroutine(PlayVoiceAndWait(intro1_Welcome));

        if (stemTexts.Length > 0 && stemTexts[0] != null) stemTexts[0].SetActive(true);
        yield return StartCoroutine(PlayVoiceAndWait(intro2_Science));

        if (stemTexts.Length > 1 && stemTexts[1] != null) stemTexts[1].SetActive(true);
        yield return StartCoroutine(PlayVoiceAndWait(intro3_Tech));

        if (stemTexts.Length > 2 && stemTexts[2] != null) stemTexts[2].SetActive(true);
        yield return StartCoroutine(PlayVoiceAndWait(intro4_Engineering));

        if (stemTexts.Length > 3 && stemTexts[3] != null) stemTexts[3].SetActive(true);
        yield return StartCoroutine(PlayVoiceAndWait(intro5_Math));

        yield return StartCoroutine(PlayVoiceAndWait(intro6_Ending));
    }

    // Hàm hỗ trợ: Phát xong file âm thanh, đợi đúng bằng độ dài file rồi mới đi tiếp
    IEnumerator PlayVoiceAndWait(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.clip = clip;
            audioSource.Play();

            // Đợi hết file âm thanh + nghỉ nửa giây cho tự nhiên
            yield return new WaitForSeconds(clip.length + 0.5f);
        }
    }
}