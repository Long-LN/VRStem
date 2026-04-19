using UnityEngine;

public class PanelAudioPlayer : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip audioClip;

    void OnEnable()
    {
        if (audioSource != null && audioClip != null)
        {
            audioSource.Stop();
            audioSource.clip = audioClip;
            audioSource.Play();
        }
    }
}