using UnityEngine;

public class SmarthomeUITabController : MonoBehaviour
{
    [Header("Kéo thả 3 Panel nội dung vào đây")]
    public GameObject[] tabPanels; 

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
    }
}