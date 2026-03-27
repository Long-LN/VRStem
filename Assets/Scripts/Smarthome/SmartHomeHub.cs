using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SmartHomeHub : MonoBehaviour
{
    public static SmartHomeHub Instance; 

    [Header("Cấu hình Giao diện (Đa màn hình)")]
    [Tooltip("Kéo thả TẤT CẢ các bảng Content (Tay trái, TV phòng khách...) vào đây")]
    public List<Transform> uiContainers; // THAY ĐỔI: Dùng List để hỗ trợ nhiều màn hình
    // THÊM: Kéo thả Category_Prefab vào đây
    public GameObject categoryPrefab;
    public GameObject buttonPrefab;

    // THÊM: Màu sắc trạng thái nút bấm
    public Color colorOn = Color.green;
    public Color colorOff = new Color(1f, 0.3f, 0.3f); // Màu đỏ nhạt

    // THAY ĐỔI: Một thiết bị giờ đây sẽ đi kèm với MỘT DANH SÁCH các nút bấm (trên nhiều màn hình)
    private Dictionary<SmartDeviceController, List<GameObject>> deviceButtons = new Dictionary<SmartDeviceController, List<GameObject>>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

public void AddDeviceToUI(SmartDeviceController newDevice)
    {
        if (deviceButtons.ContainsKey(newDevice)) return;

        List<GameObject> spawnedButtons = new List<GameObject>(); 

        foreach (Transform container in uiContainers)
        {
            if (container == null) continue;

            // 1. Lấy tên nhóm từ thiết bị (VD: "Đèn") và tạo tên ID cho nhóm (VD: "Group_Đèn")
            string catName = newDevice.categoryName;
            string groupID = "Group_" + catName;

            // 2. Tìm xem trong màn hình này đã có "Group_Đèn" chưa?
            Transform groupTransform = container.Find(groupID);

            // 3. Nếu chưa có -> Tự động in ra một cái Category_Prefab mới
            if (groupTransform == null)
            {
                GameObject newGroupObj = Instantiate(categoryPrefab, container);
                newGroupObj.name = groupID; // Đổi tên để lần sau code còn tìm thấy
                newGroupObj.transform.localScale = Vector3.one;

                // Cập nhật chữ tiêu đề (Viết hoa toàn bộ cho đẹp)
                TextMeshProUGUI titleTxt = newGroupObj.GetComponentInChildren<TextMeshProUGUI>();
                if (titleTxt != null) titleTxt.text = "- " + catName.ToUpper() + " -";

                groupTransform = newGroupObj.transform;
            }

            // 4. Tìm cái túi chứa "SubContainer" nằm bên trong cái Nhóm đó
            Transform subContainer = groupTransform.Find("SubContainer");
            if (subContainer == null) subContainer = groupTransform; // Phòng hờ lỗi

            // 5. In NÚT BẤM CỦA THIẾT BỊ vào bên trong cái "SubContainer"
            GameObject newRowObj = Instantiate(buttonPrefab, subContainer);
            newRowObj.transform.localScale = Vector3.one; 
            
            // 1. Tìm và gán Tên thiết bị
            TextMeshProUGUI nameTxt = newRowObj.transform.Find("Txt_Name").GetComponent<TextMeshProUGUI>();
            if (nameTxt != null) nameTxt.text = newDevice.deviceName; 

            // 2. Tìm nút bấm và gán sự kiện
            Transform toggleBtnTransform = newRowObj.transform.Find("Btn_Toggle");
            Button uiButton = toggleBtnTransform.GetComponent<Button>();
            
            if (uiButton != null) 
            {
                // Khi bấm nút: Đổi trạng thái thiết bị -> Sau đó cập nhật màu UI
                uiButton.onClick.AddListener(() => {
                    newDevice.ToggleDevice();
                    UpdateUIForDevice(newDevice); 
                });
            }

            spawnedButtons.Add(newRowObj);
        }

        deviceButtons.Add(newDevice, spawnedButtons);
        UpdateUIForDevice(newDevice);
    }

    // HÀM MỚI: dùng để quét và đổi màu nút của một thiết bị
    public void UpdateUIForDevice(SmartDeviceController device)
    {
        if (deviceButtons.ContainsKey(device))
        {
            foreach (GameObject rowObj in deviceButtons[device])
            {
                if (rowObj == null) continue;

                Transform toggleBtnTransform = rowObj.transform.Find("Btn_Toggle");
                if (toggleBtnTransform != null)
                {
                    Image btnImage = toggleBtnTransform.GetComponent<Image>();
                    TextMeshProUGUI stateTxt = toggleBtnTransform.Find("Txt_State").GetComponent<TextMeshProUGUI>();

                    if (device.isOn)
                    {
                        btnImage.color = colorOn;
                        stateTxt.text = "Bật";
                    }
                    else
                    {
                        btnImage.color = colorOff;
                        stateTxt.text = "Tắt";
                    }
                }
            }
        }
    }
    public void RemoveDeviceFromUI(SmartDeviceController device)
    {
        if (deviceButtons.ContainsKey(device))
        {
            // Đi qua tất cả các màn hình và xóa sạch các nút của thiết bị này
            foreach (GameObject btn in deviceButtons[device])
            {
                if (btn != null) Destroy(btn);
            }
            deviceButtons.Remove(device);   
        }
    }
}