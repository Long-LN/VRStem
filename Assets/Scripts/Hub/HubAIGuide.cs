using UnityEngine;
using System.Collections;

public class HubAIGuide : MonoBehaviour
{
    [Header("Audio Components")]
    public AudioSource audioSource;

    [Header("Audio Clips - Part 1")]
    public AudioClip voiceIntroHub1; // "Xin chào! Rất vui được gặp bạn..."
    public AudioClip voiceIntroHub2; // "Từ căn phòng này..."
    public AudioClip voiceIntroHub3; // "Hãy nhấn nút BẮT ĐẦU..."

    [Header("Audio Clips - Part 2")]
    public AudioClip voiceIntroHub4; // "Mọi thứ đã sẵn sàng!"
    public AudioClip voiceIntroHub5; // "Hôm nay bạn muốn khám phá điều gì..."

    private Coroutine currentCoroutine;

    void Start()
    {
        
    }

    // ================= PHẦN 1: CHÀO HỎI & BẤM START =================
    public void StartIntroPart1()
    {
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(IntroPart1Flow());
    }

    IEnumerator IntroPart1Flow()
    {
        yield return SpawnEffect(); // Hiệu ứng AI xuất hiện

        yield return PlayVoiceLine(voiceIntroHub1);
        yield return PlayVoiceLine(voiceIntroHub2);
        yield return PlayVoiceLine(voiceIntroHub3);

        // AI sẽ im lặng chờ người chơi bấm nút...
    }

    // ================= PHẦN 2: CHỌN BÀI HỌC =================
    // Hàm này sẽ được gọi KHI NGƯỜI CHƠI BẤM NÚT START
    public void StartIntroPart2()
    {
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(IntroPart2Flow());
    }

    IEnumerator IntroPart2Flow()
    {
        yield return PlayVoiceLine(voiceIntroHub4);
        yield return PlayVoiceLine(voiceIntroHub5);
    }

    // ================= HELPER PHÁT ÂM THANH =================
    public IEnumerator PlayVoiceLine(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            // Gắn file âm thanh vào máy phát và bấm Play
            audioSource.clip = clip;
            audioSource.Play();

            // Đợi đúng bằng thời lượng của file âm thanh rồi mới chạy tiếp
            yield return new WaitForSeconds(clip.length);

            // Nghỉ nửa giây giữa các câu để giọng nói tự nhiên hơn
            yield return new WaitForSeconds(0.5f);
        }
        else
        {
            // Nếu bạn quên chưa kéo thả file âm thanh, hệ thống sẽ đợi tạm 2 giây để không bị lỗi
            yield return new WaitForSeconds(2f);
        }
    }

    // Hiệu ứng scale phóng to AI lúc mới xuất hiện
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
}