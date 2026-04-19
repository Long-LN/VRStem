using UnityEngine;

public class SceneTrigger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public DialogueSequenceSO introSequence;
    public DialogueSequenceSO outroSequence;

    public void PlayIntro()
    {
        DialogueManager.Instance.PlaySequence(introSequence);
    }

    public void PlayOutro()
    {
        DialogueManager.Instance.PlaySequence(outroSequence);
    }

    public void StopAudio()
    {
        
    }
}
