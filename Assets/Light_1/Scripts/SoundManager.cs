using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public AudioSource audioSource;
    public AudioClip successSound;

    void Awake()
    {
        Instance = this;
    }

    public void PlaySuccess()
    {
        audioSource.PlayOneShot(successSound);
    }
}