using System.Collections;
using UnityEngine;

public class Instruct : MonoBehaviour
{
    public AudioClip[] clips;
    
    [Header("Trạng thái")]
    public int currentIndex = 0;
    public bool isPlayingPlaylist = false;
    
    void Start()
    {
        // Nếu muốn tự động chạy ngay khi vào Game
        if (clips.Length > 0)
        {
            StartCoroutine(PlayPlaylistRoutine());
        }
        
    }
    
    IEnumerator PlayPlaylistRoutine()
    {
        AudioSource audioSource = AudioManager.instance.speakSource;
        isPlayingPlaylist = true;

        while (currentIndex < clips.Length)
        {
            // 1. Gán clip hiện tại vào AudioSource
            audioSource.clip = clips[currentIndex];

            // 2. Phát âm thanh
            audioSource.Play();
            Debug.Log("Đang phát: " + clips[currentIndex].name);

            // 3. Đợi cho đến khi AudioSource phát xong clip hiện tại
            // audioSource.isPlaying sẽ trả về false khi clip chạy hết
            while (audioSource.isPlaying)
            {
                yield return null; // Chờ đến khung hình tiếp theo để kiểm tra lại
            }

            // 4. Có thể thêm một khoảng nghỉ ngắn giữa các clip nếu muốn
            yield return new WaitForSeconds(0.5f);

            // 5. Tăng chỉ số để sang bài tiếp theo
            currentIndex++;
        }

        isPlayingPlaylist = false;
        Debug.Log("Đã phát hết danh sách!");
    }
}
