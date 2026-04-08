using UnityEngine;

public class TempAudioManager : MonoBehaviour
{
    public static TempAudioManager Instance;

    [Header("Background")]
    public AudioSource bgMusic;

    [Header("SFX")]
    public AudioClip moleculeSpawnSound;   // âm khi spawn phân tử
    public AudioClip moleculeHitSound;     // âm khi phân tử va chạm
    public AudioClip moleculeGrabSound;
    public AudioClip boxGrabSound;         // âm khi grab hộp
    public AudioClip temperatureUpSound;   // âm khi nhiệt độ tăng
    public AudioClip balanceSound;         // âm khi cân bằng nhiệt

    AudioSource sfxSource;

    void Awake()
    {
        // Singleton
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.spatialBlend = 0f; // 2D
        sfxSource.volume = 0.8f;
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, volume);
    }

    public void SetBGVolume(float vol)
    {
        if (bgMusic != null)
            bgMusic.volume = vol;
    }

    public void PauseBG() { if (bgMusic != null) bgMusic.Pause(); }
    public void ResumeBG() { if (bgMusic != null) bgMusic.Play(); }
}