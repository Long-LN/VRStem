using UnityEngine;

public class SocketHighlighter : MonoBehaviour
{
    [Header("--- Ổ cắm này dành cho loại nào? ---")]
    [Tooltip("Phải gõ CHÍNH XÁC giống chữ bên thiết bị (VD: Ceiling, Wall)")]
    public string acceptedSocketType = "Ceiling";

    [Header("--- Hình ảnh Phát sáng ---")]
    public GameObject highlightVisual;

    void OnEnable()
    {
        if (highlightVisual != null) highlightVisual.SetActive(false);

        // Bật máy nghe đài khi ổ cắm được xuất hiện
        DeviceGrabBroadcaster.OnAnyDeviceGrabbed += HandleDeviceGrabbed;
        DeviceGrabBroadcaster.OnAnyDeviceReleased += HandleDeviceReleased;
    }

    void OnDisable()
    {
        // Tắt máy nghe đài ngay khi ổ cắm bị ẩn đi hoặc bị xóa
        DeviceGrabBroadcaster.OnAnyDeviceGrabbed -= HandleDeviceGrabbed;
        DeviceGrabBroadcaster.OnAnyDeviceReleased -= HandleDeviceReleased;
    }

    // Hàm này tự động chạy khi có BẤT KỲ thiết bị nào trong nhà được cầm lên
    private void HandleDeviceGrabbed(string grabbedType)
    {
        // Kiểm tra xem món đồ người ta đang cầm có khớp với ổ cắm này không?
        if (grabbedType == acceptedSocketType)
        {
            if (highlightVisual != null) highlightVisual.SetActive(true); // Bật sáng!
        }
    }

    // Hàm này tự động chạy khi buông tay
    private void HandleDeviceReleased()
    {
        if (highlightVisual != null) highlightVisual.SetActive(false); // Tắt sáng!
    }
}