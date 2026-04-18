using UnityEngine;
using System.Collections;
using TMPro;

public class HubAIGuide : MonoBehaviour
{
    [Header("Audio Components")]
    public AudioSource audioSource;

    [Header("Audio Clips - Part 1")]
    public AudioClip voiceIntroHub1;
    public AudioClip voiceIntroHub2;
    public AudioClip voiceIntroHub3;

    [Header("Audio Clips - Part 2")]
    public AudioClip voiceIntroHub4;
    public AudioClip voiceIntroHub5;

    [Header("UI Elements (Hiệu ứng chữ STEM)")]
    public GameObject stemPanel;
    public TextMeshProUGUI[] stemTexts;
    public float fadeSpeed = 2f;
    public float delayBetweenLines = 0.5f;

    private Coroutine currentCoroutine;

    void Start()
    {
        if (stemPanel != null) stemPanel.SetActive(false);
    }

    // ================= PHẦN 1: CHÀO HỎI & BẤM START =================
    public void StartIntroPart1()
    {
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(IntroPart1Flow());
    }

    IEnumerator IntroPart1Flow()
    {
        yield return SpawnEffect();

        yield return PlayVoiceLine(voiceIntroHub1);

        yield return PlayVoiceLine(voiceIntroHub2);

        if (stemPanel != null) stemPanel.SetActive(true);
        StartCoroutine(RevealSTEMTexts());

        yield return PlayVoiceLine(voiceIntroHub3);

        // AI sẽ im lặng chờ người chơi bấm nút...
    }

    // Hiệu ứng Typewriter: Hiện chữ từ trái qua phải
    IEnumerator RevealSTEMTexts()
    {
        // BƯỚC 1: Giấu toàn bộ chữ ngay từ đầu
        foreach (var txt in stemTexts)
        {
            if (txt != null)
            {
                // Đảm bảo màu chữ hiển thị rõ (phòng trường hợp bản code cũ đang để Alpha = 0)
                Color c = txt.color;
                c.a = 1f;
                txt.color = c;

                // Set số ký tự nhìn thấy được về 0
                txt.maxVisibleCharacters = 0;
            }
        }

        // BƯỚC 2: Cho chữ chạy ra từ trái qua phải từng dòng một
        foreach (var txt in stemTexts)
        {
            if (txt != null)
            {
                // Bắt Unity tính toán nội dung trước để biết chính xác có bao nhiêu ký tự
                txt.ForceMeshUpdate();
                int totalCharacters = txt.textInfo.characterCount;

                // Tăng dần số ký tự hiển thị từ 0 cho đến hết câu
                for (int i = 0; i <= totalCharacters; i++)
                {
                    txt.maxVisibleCharacters = i;

                    // Tốc độ đánh máy (Bạn có thể tăng biến fadeSpeed ngoài Inspector để chữ chạy nhanh hơn)
                    yield return new WaitForSeconds(1f / (fadeSpeed * 20f));
                }

                // Đợi một chút (nghỉ nhịp) rồi mới chạy dòng chữ tiếp theo
                yield return new WaitForSeconds(delayBetweenLines);
            }
        }
    }

    // ================= PHẦN 2: CHỌN BÀI HỌC =================
    public void StartIntroPart2()
    {
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(IntroPart2Flow());
    }

    IEnumerator IntroPart2Flow()
    {
        // (Tùy chọn) Nếu bạn muốn tắt bảng STEM đi khi AI nói tiếp thì bật dòng này:
        // if (stemPanel != null) stemPanel.SetActive(false); 

        yield return PlayVoiceLine(voiceIntroHub4);
        yield return PlayVoiceLine(voiceIntroHub5);
    }

    // ================= HELPER PHÁT ÂM THANH =================
    public IEnumerator PlayVoiceLine(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
            yield return new WaitForSeconds(clip.length);
            yield return new WaitForSeconds(0.5f);
        }
        else
        {
            yield return new WaitForSeconds(2f);
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
}