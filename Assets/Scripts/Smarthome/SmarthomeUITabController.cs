using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SmarthomeUITabController : MonoBehaviour
{
    [Header("Kéo thả 3 Panel nội dung vào đây")]
    public GameObject[] tabPanels; 

    // THÊM MỚI: Danh sách các Nút bấm để đổi màu
    [Header("--- Danh sách các Nút (Button) ---")]
    [Tooltip("Kéo thả lần lượt 3 cái nút Thiết bị, Điều khiển, Camera")]
    public Image[] tabButtons;

    [Header("--- Cấu hình Màu sắc ---")]
    public Color activeColor = new Color(0.7f, 0.9f, 1f); // Màu xanh nhạt khi được chọn
    public Color inactiveColor = Color.white;             // Màu trắng khi không chọn

    void Start()
    {
        // Khi vừa mở UI lên, mặc định hiển thị Tab số 0 (Spawn), ẩn các tab khác
        SwitchTab(0);
    }

    public void SwitchTab(int tabIndex)
    {
        // 1. Bật/Tắt Panels
        for (int i = 0; i < tabPanels.Length; i++)
        {
            if (tabPanels[i] != null) tabPanels[i].SetActive(i == tabIndex);
        }

        // 2. Đổi màu nút
        for (int i = 0; i < tabButtons.Length; i++)
        {
            if (tabButtons[i] != null)
            {
                tabButtons[i].color = (i == tabIndex) ? activeColor : inactiveColor;
            }
        }

        // 3. THAY ĐỔI: Thay vì gọi trực tiếp, chúng ta gọi một Coroutine để trì hoãn 1 khung hình
        if (tabIndex == 2 && SecurityCameraManager.Instance != null)
        {
            StartCoroutine(DelayUpdateCameras());
        }
    }

    // HÀM MỚI: Tiến trình chờ đợi
    private IEnumerator DelayUpdateCameras()
    {
        // Ra lệnh cho code tạm dừng lại, chờ đến khi Unity vẽ xong toàn bộ UI của khung hình hiện tại
        yield return new WaitForEndOfFrame(); 
        
        // Sau khi mọi thứ đã "tỉnh ngủ", mới ra lệnh cập nhật Camera
        SecurityCameraManager.Instance.UpdateAllDisplays();
    }
}