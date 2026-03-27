using UnityEngine;
using UnityEngine.UI;

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

    // Hàm này sẽ được gọi khi bạn bấm vào các nút Tab trên UI
    public void SwitchTab(int tabIndex)
    {
        for (int i = 0; i < tabPanels.Length; i++)
        {
            // Nếu số thứ tự của vòng lặp khớp với số thứ tự của Tab được chọn -> Bật nó lên
            if (i == tabIndex)
            {
                tabPanels[i].SetActive(true);
            }
            // Nếu không khớp -> Tắt nó đi
            else
            {
                tabPanels[i].SetActive(false);
            }
        }

        // 2. THÊM MỚI: Đổi màu nền của các Nút Tab
        for (int i = 0; i < tabButtons.Length; i++)
        {
            if (tabButtons[i] != null)
            {
                // Nếu đúng là nút đang chọn -> Tô màu Active. Nếu không -> Tô màu Inactive
                tabButtons[i].color = (i == tabIndex) ? activeColor : inactiveColor;
            }
        }
    }
}