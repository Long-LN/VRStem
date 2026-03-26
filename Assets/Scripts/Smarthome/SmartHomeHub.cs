using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SmartHomeHub : MonoBehaviour
{
    public static SmartHomeHub Instance; 

    [Header("Cấu hình Giao diện (Đa màn hình)")]
    [Tooltip("Kéo thả TẤT CẢ các bảng Content (Tay trái, TV phòng khách...) vào đây")]
    public List<Transform> deviceListContainers; // THAY ĐỔI: Dùng List để hỗ trợ nhiều màn hình
    public GameObject buttonPrefab;

    // THAY ĐỔI: Một thiết bị giờ đây sẽ đi kèm với MỘT DANH SÁCH các nút bấm (trên nhiều màn hình)
    private Dictionary<SmartDeviceController, List<GameObject>> deviceButtons = new Dictionary<SmartDeviceController, List<GameObject>>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void AddDeviceToUI(SmartDeviceController newDevice)
    {
        if (deviceButtons.ContainsKey(newDevice)) return;

        List<GameObject> spawnedButtons = new List<GameObject>(); // Tạo danh sách nút tạm thời

        // VÒNG LẶP: Đi qua từng cái màn hình (Tay trái, TV,...) để in nút
        foreach (Transform container in deviceListContainers)
        {
            if (container == null) continue;

            GameObject newButtonObj = Instantiate(buttonPrefab, container);
            newButtonObj.transform.localScale = Vector3.one; // Chống teo nhỏ trong VR
            
            TextMeshProUGUI btnText = newButtonObj.GetComponentInChildren<TextMeshProUGUI>();
            if (btnText != null) btnText.text = "BẬT/TẮT " + newDevice.deviceName; 

            Button uiButton = newButtonObj.GetComponent<Button>();
            if (uiButton != null) uiButton.onClick.AddListener(() => newDevice.ToggleDevice());

            spawnedButtons.Add(newButtonObj); // Lưu cái nút vừa in vào danh sách tạm
        }

        // Ghi vào sổ tay: Thiết bị này đang sở hữu những cái nút nào trên các màn hình
        deviceButtons.Add(newDevice, spawnedButtons);
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