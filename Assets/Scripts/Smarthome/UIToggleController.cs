using UnityEngine;
using UnityEngine.InputSystem; // Bắt buộc phải có thư viện này để nhận nút bấm VR

public class UIToggleController : MonoBehaviour
{
    [Header("Bảng UI cần ẩn/hiện")]
    public GameObject uiPanel; 

    [Header("Nút bấm trên tay cầm")]
    [Tooltip("Chọn nút bạn muốn dùng để bật/tắt (VD: Primary Button tay trái)")]
    public InputActionReference toggleAction;

    // Khi Script được bật, bắt đầu lắng nghe sự kiện bấm nút
    private void OnEnable()
    {
        toggleAction.action.performed += ToggleUI;
    }

    // Khi Script bị tắt, ngừng lắng nghe để tránh lỗi bộ nhớ
    private void OnDisable()
    {
        toggleAction.action.performed -= ToggleUI;
    }

    // Hàm thực thi việc đảo ngược trạng thái bật/tắt
    private void ToggleUI(InputAction.CallbackContext context)
    {
        if (uiPanel != null)
        {
            // activeSelf là trạng thái hiện tại. Dấu ! nghĩa là phủ định (đang bật thành tắt, tắt thành bật)
            bool isActive = uiPanel.activeSelf;
            uiPanel.SetActive(!isActive);
        }
    }
}