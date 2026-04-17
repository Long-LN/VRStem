using System.Collections;
using UnityEngine;

public class SequenceSoundManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] playlist;
    public static bool skipIntro = false;

    private int currentIndex = 0;

    private bool hasStartedEnvironment = false;

    void Start()
    {
        if (skipIntro)
        {
            currentIndex = 3; 
            skipIntro = false;
        }
        else
        {
            currentIndex = 0;
            hasStartedEnvironment = false;
        }

        StartCoroutine(PlaySequence());
    }

    public void TriggerStartButton()
    {
        hasStartedEnvironment = true;
    }

    IEnumerator PlaySequence()
    {
        while (currentIndex < playlist.Length)
        {
            audioSource.clip = playlist[currentIndex];
            audioSource.Play();

            yield return new WaitForSeconds(audioSource.clip.length);

            currentIndex++;

            //  CHỈ PAUSE SAU INTRO nếu chưa bấm nút
            if (!hasStartedEnvironment && currentIndex >= 2)
            {
                yield return new WaitUntil(() => hasStartedEnvironment);
            }
        }
    }

    public void StartFromStep(int index)
    {
        currentIndex = index;
        StopAllCoroutines();
        StartCoroutine(PlaySequence());
    }
}