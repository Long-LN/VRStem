using UnityEngine;

public class SceneTrigger : MonoBehaviour
{
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
}
