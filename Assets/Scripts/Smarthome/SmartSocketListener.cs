using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor))] 
public class SmartSocketListener : MonoBehaviour
{
    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socket;

    void Awake()
    {
        socket = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor>();
    }

    void OnEnable()
    {
        // Lắng nghe khi CẮM VÀO
        socket.selectEntered.AddListener(OnDeviceConnected);
        // Lắng nghe khi RÚT RA
        socket.selectExited.AddListener(OnDeviceDisconnected); 
    }

    void OnDisable()
    {
        socket.selectEntered.RemoveListener(OnDeviceConnected);
        socket.selectExited.RemoveListener(OnDeviceDisconnected);
    }

    private void OnDeviceConnected(SelectEnterEventArgs args)
    {
        SmartDeviceController device = args.interactableObject.transform.GetComponent<SmartDeviceController>();
        if (device != null && SmartHomeHub.Instance != null)
        {
            SmartHomeHub.Instance.AddDeviceToUI(device);
        }
    }

    // HÀM MỚI: Gọi khi vật thể bị rút ra khỏi ổ điện
    private void OnDeviceDisconnected(SelectExitEventArgs args)
    {
        SmartDeviceController device = args.interactableObject.transform.GetComponent<SmartDeviceController>();
        
        // Báo cho Hub biết để xóa nút trên UI
        if (device != null && SmartHomeHub.Instance != null)
        {
            // (Tùy chọn) Tắt thiết bị đi nếu nó đang bật
            if (device.isOn) device.ToggleDevice(); 

            SmartHomeHub.Instance.RemoveDeviceFromUI(device);
        }
    }
}