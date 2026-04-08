using UnityEngine;
using System;

[Serializable]
public class DialogueSegment // Một lớp nhỏ định nghĩa từng mảnh hội thoại
{
    [TextArea(2, 5)]
    public string subTitle;
    public AudioClip voiceClip;
    public float delayAfter = 1.0f; // Thời gian nghỉ sau khi đoạn này kết thúc
    
}

[CreateAssetMenu(fileName = "NewDialogueSequence", menuName = "Dialogue/Sequence")]
public class DialogueSequenceSO : ScriptableObject
{
    public DialogueSegment[] segments; // Danh sách các đoạn thoại
    public bool playOnlyOnce = false;
    
    [NonSerialized] // Quan trọng: Không lưu trạng thái này vào ổ cứng khi tắt Game
    public bool hasBeenPlayed = false;

    // Reset trạng thái khi bắt đầu game (tùy chọn)
    private void OnEnable() { hasBeenPlayed = false; }
}