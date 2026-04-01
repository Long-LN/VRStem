using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }
    
    [SerializeField] public AudioSource speakSource;
    [SerializeField] private AudioSource effectSource;

    private void Awake()
    {
        instance = this;
    }

    public void PlaySpeak(AudioClip clip)
    {
        speakSource.PlayOneShot(clip);
    }

    public void PlayEffect(AudioClip clip)
    {
        effectSource.PlayOneShot(clip);
    }
}
