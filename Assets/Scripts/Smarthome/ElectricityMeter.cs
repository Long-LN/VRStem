using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ElectricityMeter : MonoBehaviour
{
    public static ElectricityMeter Instance;

    [Header("--- Thông số Hiển thị ---")]
    // THAY ĐỔI: Dùng List để cập nhật tiền điện lên nhiều màn hình cùng lúc
    public List<TextMeshProUGUI> meterTexts;

    [Header("--- Cấu hình Điện ---")]
    public float electricityPrice = 3500f; // Đơn giá VNĐ / 1 kWh
    
    [Tooltip("Hệ số thời gian: 3600 nghĩa là 1 giây đời thực = 1 giờ trong game")]
    public float timeScale = 3600f; 

    // Các biến lưu trữ dữ liệu realtime
    private float totalActiveWatts = 0f; // Tổng công suất đang bật
    private float accumulatedKWh = 0f;   // Số điện đã dùng

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    // Các thiết bị sẽ gọi hàm này khi BẬT
    public void AddLoad(float watts)
    {
        totalActiveWatts += watts;
    }

    // Các thiết bị sẽ gọi hàm này khi TẮT
    public void RemoveLoad(float watts)
    {
        totalActiveWatts -= watts;
        if (totalActiveWatts < 0) totalActiveWatts = 0; // Tránh lỗi số âm
    }

    void Update()
    {
        // Tính toán thời gian ảo đã trôi qua trong 1 khung hình (đơn vị: Giờ)
        float virtualHoursPassed = (Time.deltaTime * timeScale) / 3600f;

        // Công thức Vật lý: E = P * t (Lưu ý: P phải chia 1000 để đổi từ W sang kW)
        accumulatedKWh += (totalActiveWatts / 1000f) * virtualHoursPassed;

        // Tính tiền
        float totalCost = accumulatedKWh * electricityPrice;

        string displayText = $"ĐANG TIÊU THỤ: {totalActiveWatts} W\n" +
                             $"SỐ ĐIỆN: {accumulatedKWh:F2} kWh\n" +
                             $"TỔNG TIỀN: {totalCost:N0} VNĐ";

        // VÒNG LẶP: In con số này lên TẤT CẢ các màn hình (Tay trái, TV...)
        foreach (TextMeshProUGUI txt in meterTexts)
        {
            if (txt != null) txt.text = displayText;
        }
    }
}