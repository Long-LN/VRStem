using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
public class DeviceGrabBroadcaster : MonoBehaviour
{
    [Header("--- Phân loại Thiết bị ---")]
    public string deviceSocketType = "Ceiling";

    public static Action<string> OnAnyDeviceGrabbed;
    public static Action OnAnyDeviceReleased;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;

    void Awake()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grabInteractable == null) Debug.LogError($"[Broadcaster] {gameObject.name} không có XRGrabInteractable!");
    }

    void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);
    }

    void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        grabInteractable.selectExited.RemoveListener(OnReleased);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        // TRẠM GÁC 1: Báo cáo đã cầm lên
        Debug.Log($"[Broadcaster] Đã CẦM thiết bị: {gameObject.name} | Gửi đi từ khóa: '{deviceSocketType}'");
        
        if (OnAnyDeviceGrabbed == null)
        {
            Debug.LogWarning("[Broadcaster] Cảnh báo: Kênh phát thanh đang trống, KHÔNG CÓ Ổ CẮM NÀO đang nghe!");
        }
        else
        {
            OnAnyDeviceGrabbed.Invoke(deviceSocketType);
        }
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        // TRẠM GÁC 2: Báo cáo buông tay
        Debug.Log($"[Broadcaster] Đã BUÔNG thiết bị: {gameObject.name}");
        OnAnyDeviceReleased?.Invoke();
    }

    // Hàm dọn dẹp
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStaticEvents()
    {
        // Cắt đứt mọi liên lạc của các ổ cắm cũ từ lần Play trước
        OnAnyDeviceGrabbed = null;
        OnAnyDeviceReleased = null;
    }
}