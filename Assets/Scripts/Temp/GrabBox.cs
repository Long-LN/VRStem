using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class GrabBox : MonoBehaviour
{
    XRGrabInteractable grab;
    HeatBox heatBox;

    void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
        heatBox = GetComponent<HeatBox>();

        grab.selectEntered.AddListener(_ => OnGrabbed());
        grab.selectExited.AddListener(_ => OnReleased());
    }

    void OnGrabbed()
    {
        TempAudioManager.Instance?.PlaySFX(TempAudioManager.Instance.boxGrabSound);
        // Tạm dừng truyền nhiệt
        if (heatBox != null)
            heatBox.SetPaused(true);

        // Phân tử đã là con của hộp → tự di chuyển theo khi grab
        // Không kinematic, không SetParent → giữ nguyên tốc độ
    }

    void OnReleased()
    {
        // Bật lại truyền nhiệt
        if (heatBox != null)
            heatBox.SetPaused(false);
    }
}