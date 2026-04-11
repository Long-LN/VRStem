using UnityEngine;
using UnityEngine.UI; // THÊM DÒNG NÀY: Để lấy RawImage
using TMPro;
using System.Collections.Generic;

public class SecurityCameraManager : MonoBehaviour
{
    public static SecurityCameraManager Instance;

    [System.Serializable]
    public class CameraScreen
    {
        public TextMeshProUGUI camNameText;
        public GameObject noSignalUI;
        // THAY ĐỔI LỚN: Kéo tấm màn hình Raw Image vào đây thay vì file cuộn băng
        public RawImage screenDisplay; 
        [HideInInspector] public int currentIndex = 0;
    }

    [Header("--- Danh sách Màn hình Camera ---")]
    public List<CameraScreen> screens;

    private List<SmartDeviceController> cameraList = new List<SmartDeviceController>();
    // TỪ ĐIỂN: Lưu trữ cuộn băng RIÊNG của từng Camera
    private Dictionary<SmartDeviceController, RenderTexture> camFeeds = new Dictionary<SmartDeviceController, RenderTexture>();

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void RegisterCamera(SmartDeviceController cam)
    {
        if (!cameraList.Contains(cam))
        {
            cameraList.Add(cam);

            // TÍNH NĂNG MỚI: Tự động tạo "Cuộn băng" (RenderTexture) cho Camera này bằng Code!
            // Bạn không cần phải tạo file bằng tay trong Project nữa.
            if (cam.securityCamera != null && !camFeeds.ContainsKey(cam))
            {
                RenderTexture newRT = new RenderTexture(512, 512, 16);
                cam.securityCamera.targetTexture = newRT;
                camFeeds.Add(cam, newRT);
            }

            UpdateAllDisplays();
        }
    }

    public void NextCamera(int screenIndex)
    {
        if (cameraList.Count == 0 || screenIndex >= screens.Count) return;
        screens[screenIndex].currentIndex++;
        if (screens[screenIndex].currentIndex >= cameraList.Count) screens[screenIndex].currentIndex = 0; 
        UpdateAllDisplays(); // Phải update tất cả để kiểm tra xem có cam nào bị bỏ trống không
    }

    public void PrevCamera(int screenIndex)
    {
        if (cameraList.Count == 0 || screenIndex >= screens.Count) return;
        screens[screenIndex].currentIndex--;
        if (screens[screenIndex].currentIndex < 0) screens[screenIndex].currentIndex = cameraList.Count - 1; 
        UpdateAllDisplays();
    }

    public void UpdateAllDisplays()
    {
        // 1. Cập nhật hình ảnh cho TỪNG MÀN HÌNH
        for (int i = 0; i < screens.Count; i++)
        {
            if (cameraList.Count == 0) continue;

            CameraScreen screen = screens[i];
            SmartDeviceController currentCam = cameraList[screen.currentIndex];

            if (screen.camNameText != null) screen.camNameText.text = currentCam.deviceName;

            if (currentCam.isOn)
            {
                // BẬT: Lấy sóng từ cuộn băng của Camera truyền lên Màn hình
                if (screen.screenDisplay != null && camFeeds.ContainsKey(currentCam))
                {
                    screen.screenDisplay.texture = camFeeds[currentCam];
                }
                if (screen.noSignalUI != null) screen.noSignalUI.SetActive(false);
            }
            else
            {
                // TẮT: Ngắt sóng, hiện màn đen No Signal
                if (screen.screenDisplay != null) screen.screenDisplay.texture = null;
                if (screen.noSignalUI != null) screen.noSignalUI.SetActive(true);
            }
        }

        // 2. TỐI ƯU FPS: Quét xem Camera nào ĐANG BẬT ĐIỆN nhưng KHÔNG CÓ AI XEM -> Tắt nó đi!
        foreach (var cam in cameraList)
        {
            if (cam.securityCamera == null) continue;
            
            bool isBeingWatched = false;
            foreach (var screen in screens)
            {
                if (cameraList[screen.currentIndex] == cam && cam.isOn)
                {
                    isBeingWatched = true;
                    break;
                }
            }
            cam.securityCamera.enabled = isBeingWatched;
        }
    }
}