using System.Collections;
using UnityEngine;

public class SequenceSoundManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] playlist;

    [Header("Giao diện Text STEM")]
    [Tooltip("Kéo 4 cục Text S-T-E-M vào đây")]
    public GameObject[] stemTexts;

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
            foreach (var txt in stemTexts)
            {
                if (txt != null)
                    txt.SetActive(false);
            }
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

            CheckAndShowText(currentIndex);
            yield return new WaitForSeconds(audioSource.clip.length);

            currentIndex++;

            //  CHỈ PAUSE SAU INTRO nếu chưa bấm nút
            if (!hasStartedEnvironment && currentIndex >= 2)
            {
                yield return new WaitUntil(() => hasStartedEnvironment);
            }
        }
    }

    void CheckAndShowText(int index)
    {
        // Theo yêu cầu của bạn: Text hiện ở index 4, 5, 6, 7
        // Chúng ta trừ đi 4 để khớp với mảng stemTexts (0, 1, 2, 3)
        int textIndex = index - 4;

        if (textIndex >= 0 && textIndex < stemTexts.Length)
        {
            if (stemTexts[textIndex] != null)
            {
                stemTexts[textIndex].SetActive(true);
                Debug.Log("Đang hiện Text tại Index: " + index);
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
