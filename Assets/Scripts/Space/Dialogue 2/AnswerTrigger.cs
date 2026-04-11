using System;
using System.Collections;
using UnityEngine;

public class AnswerTrigger : MonoBehaviour
{
    public static AnswerTrigger Instance;

    [Serializable]
    public class PlanetDialogue
    {
        public string planetName;
        public DialogueSequenceSO descriptionSequence;
    }

    [Header("Dialogue mô tả từng hành tinh")]
    public PlanetDialogue[] planetDialogues;

    private void Awake() => Instance = this;

    public void PlayDescription(string planetName, Action onFinished = null)
    {
        PlanetDialogue entry = Array.Find(planetDialogues, p => p.planetName == planetName);
        if (entry == null || entry.descriptionSequence == null)
        {
            Debug.LogWarning($"[AnswerTrigger] Không tìm thấy sequence cho: {planetName}");
            onFinished?.Invoke();
            return;
        }

        Debug.Log($"[AnswerTrigger] Bắt đầu phát mô tả: {planetName}");
        DialogueManager.Instance.PlaySequence(entry.descriptionSequence);
        StartCoroutine(WaitForSequenceFinished(onFinished));
    }

    private IEnumerator WaitForSequenceFinished(Action onFinished)
    {
        Debug.Log("[AnswerTrigger] Đợi audio bắt đầu phát...");

        // Đợi tối đa 1 giây để audio bắt đầu
        float timeout = 1f;
        float elapsed = 0f;
        while (!DialogueManager.Instance.audioSource.isPlaying && elapsed < timeout)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (!DialogueManager.Instance.audioSource.isPlaying)
        {
            Debug.LogWarning("[AnswerTrigger] Audio không bắt đầu phát sau 1 giây!");
            onFinished?.Invoke();
            yield break;
        }

        Debug.Log("[AnswerTrigger] Audio đang phát, đợi xong...");
        yield return new WaitWhile(() => DialogueManager.Instance.audioSource.isPlaying);

        Debug.Log("[AnswerTrigger] Audio xong, gọi callback!");
        onFinished?.Invoke();
    }
}