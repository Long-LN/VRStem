using UnityEngine;
using System.Collections.Generic; // THÊM DÒNG NÀY ĐỂ DÙNG DICTIONARY

public class SmartDeviceController : MonoBehaviour
{
    [Header("--- Thông tin Thiết bị ---")]
    public string deviceName = "Thiết bị thông minh";
    
    // THÊM: Nhãn phân loại nhóm (VD: Đèn, Quạt, An Ninh, Cửa...)
    [Tooltip("Gõ tên nhóm để gom các thiết bị giống nhau lại (VD: Đèn, Quạt)")]
    public string categoryName = "Khác";

    [Tooltip("Công suất tiêu thụ (Watt)")]
    public float powerRating = 50f;

    // THÊM MỚI: Đánh dấu thiết bị này là loại gắn sẵn âm tường (không cần ổ cắm)
    [Tooltip("Tick vào nếu thiết bị này được gắn sẵn (không cần ổ cắm)")]
    public bool isHardwired = false;

    // THÊM MỚI: Cuốn sổ "dùng chung" ghi nhớ số lượng của từng loại thiết bị
    // Chữ 'static' vô cùng quan trọng ở đây!
    private static Dictionary<string, int> deviceTypeCounters = new Dictionary<string, int>();

    [Header("--- Trạng thái ---")]
    public bool isOn = false;

    [Header("--- Cấu hình Đèn ---")]
    public Light deviceLight; 
    public Renderer bulbRenderer; 
    [ColorUsage(true, true)] public Color glowColor = Color.white; 
    private Material bulbMaterial;

    [Header("--- Cấu hình Quạt ---")]
    public Transform fanBlades; 
    public float spinSpeed = 1000f; 
    public Vector3 rotationAxis = new Vector3(0, 0, 1);

    [Header("--- Cấu hình Camera ---")]
    // Khai báo Camera
    public Camera securityCamera;
    [Tooltip("Text (TMP) chứa chữ NO SIGNAL")]
    // THAY ĐỔI: Chữ NO SIGNAL giờ đây phải hiển thị trên cả TV và Tay trái
    public List<GameObject> noSignalTexts;

    [Header("--- Cấu hình Cửa ---")]
    public Transform doorHinge; 
    public float openAngle = 90f;  // Góc mở (có thể điền -90 nếu muốn mở hướng ngược lại)
    public float swingSpeed = 3f;  // Tốc độ vung cửa nhanh hay chậm
    // Bộ nhớ lưu góc xoay
    private Quaternion closedRotation;
    private Quaternion openRotation;
    // THÊM MỚI: Biến quyết định cửa sẽ xoay quanh trục nào (Mặc định: Trục Y)
    [Tooltip("Trục quay")]
    public Vector3 doorRotationAxis = new Vector3(0, 1, 0); 

    [Header("--- Cấu hình TV / Màn hình UI ---")]
    [Tooltip("Kéo thả GameObject Canvas chứa UI của TV vào đây để nó ẩn/hiện")]
    public GameObject displayScreenUI;

    // HÀM MỚI: Tự động chạy ngay khoảnh khắc vật thể được Spawn ra
    void Awake()
    {
        // 1. Kiểm tra xem loại thiết bị này (VD: "Đèn Tường") đã có trong sổ đếm chưa?
        if (!deviceTypeCounters.ContainsKey(deviceName))
        {
            deviceTypeCounters[deviceName] = 1; // Chưa có thì bắt đầu đếm là 1
        }
        else
        {
            deviceTypeCounters[deviceName]++;   // Có rồi thì cộng thêm 1
        }

        // 2. Cập nhật lại tên hiển thị (VD: "Đèn Tường" + " " + "1" -> "Đèn Tường 1")
        deviceName = deviceName + " " + deviceTypeCounters[deviceName];

        // 3. (Tùy chọn) Đổi tên của cục 3D trong cửa sổ Hierarchy để dễ debug
        gameObject.name = deviceName; 

        // THÊM: Tính toán trước góc Đóng và góc Mở của cửa
        if (doorHinge != null)
        {
            closedRotation = doorHinge.localRotation;
            openRotation = closedRotation * Quaternion.Euler(doorRotationAxis * openAngle);
        }
    }

void Start()
    {
        if (bulbRenderer != null)
        {
            bulbMaterial = bulbRenderer.material;
            bulbMaterial.SetColor("_EmissionColor", Color.black); 
        }

        // THÊM MỚI: Nếu là thiết bị gắn sẵn, tự động gọi Hub để tạo nút bấm UI
        if (isHardwired && SmartHomeHub.Instance != null)
        {
            SmartHomeHub.Instance.AddDeviceToUI(this);
        }

        // THÊM MỚI: Đồng bộ trạng thái màn hình TV
        if (displayScreenUI != null) displayScreenUI.SetActive(isOn);

        // THÊM MỚI: Đảm bảo trạng thái thiết bị và đồng hồ điện đồng bộ lúc mới mở game
        if (isOn)
        {
            if (ElectricityMeter.Instance != null) ElectricityMeter.Instance.AddLoad(powerRating);
            if (deviceLight != null) deviceLight.enabled = true;
            if (bulbMaterial != null) bulbMaterial.SetColor("_EmissionColor", glowColor);

            if (securityCamera != null) securityCamera.enabled = true;
            if (noSignalTexts != null)
            {
                foreach (GameObject text in noSignalTexts)
                {
                    if (text != null) text.SetActive(false);
                }
            }
        }
        else 
        {
            if (securityCamera != null) securityCamera.enabled = false;
            if (noSignalTexts != null)
            {
                foreach (GameObject text in noSignalTexts)
                {
                    if (text != null) text.SetActive(true);
                }
            }
        }
    }

    void Update()
    {
        if (isOn && fanBlades != null)
        {
            fanBlades.Rotate(rotationAxis * spinSpeed * Time.deltaTime);
        }

        // THÊM: Nếu có bản lề, từ từ xoay nó tới góc đích (Mở hoặc Đóng)
        if (doorHinge != null)
        {
            Quaternion targetRotation = isOn ? openRotation : closedRotation;
            doorHinge.localRotation = Quaternion.Lerp(doorHinge.localRotation, targetRotation, Time.deltaTime * swingSpeed);
        }
    }

    public void ToggleDevice()
    {
        isOn = !isOn; 

        if (isOn)
        {
            if (deviceLight != null) deviceLight.enabled = true;
            if (bulbMaterial != null) bulbMaterial.SetColor("_EmissionColor", glowColor);
            
            // Bật Camera quay hình
            if (securityCamera != null) securityCamera.enabled = true;
            if (noSignalTexts != null)
            {
                foreach (GameObject text in noSignalTexts)
                {
                    if (text != null) text.SetActive(false);
                }
            }

            if (displayScreenUI != null) displayScreenUI.SetActive(true); // Bật TV -> Hiện UI

            // Báo cho đồng hồ cộng thêm điện
            if (ElectricityMeter.Instance != null) ElectricityMeter.Instance.AddLoad(powerRating);
        }
        else
        {
            if (deviceLight != null) deviceLight.enabled = false;
            if (bulbMaterial != null) bulbMaterial.SetColor("_EmissionColor", Color.black);
            
            // Xử lý tắt Camera và bôi đen màn hình
            if (securityCamera != null) 
            {
                securityCamera.enabled = false; // Dừng quay hình

                // --- THÊM MỚI: Xóa cuộn băng về màu đen ---
                if (securityCamera.targetTexture != null)
                {
                    // 1. Lưu lại trạng thái bộ nhớ hiện tại của Unity
                    RenderTexture currentRT = RenderTexture.active; 
                    
                    // 2. Trỏ thẳng vào cuộn băng của Camera này
                    RenderTexture.active = securityCamera.targetTexture; 
                    
                    // 3. Dùng thư viện Đồ họa (GL) tô đen xì toàn bộ (Clear)
                    GL.Clear(true, true, Color.black); 
                    
                    // 4. Trả lại trạng thái bộ nhớ như cũ cho Unity
                    RenderTexture.active = currentRT; 
                }
                // ------------------------------------------
                if (noSignalTexts != null)
                {
                    foreach (GameObject text in noSignalTexts)
                    {
                        if (text != null) text.SetActive(true);
                    }
                }
            }

            if (displayScreenUI != null) displayScreenUI.SetActive(false); // Tắt TV -> Giấu UI

            // Báo cho đồng hồ trừ bớt điện
            if (ElectricityMeter.Instance != null) ElectricityMeter.Instance.RemoveLoad(powerRating);
        }

        // Báo cho Hub biết trạng thái đã thay đổi để Hub tô lại màu trên mọi màn hình
        if (SmartHomeHub.Instance != null)
        {
            SmartHomeHub.Instance.UpdateUIForDevice(this);
        }
    }

    private void OnDestroy()
    {
        // Nếu thiết bị bị xóa (rút điện/vứt rác) khi đang bật, trừ điện đi
        if (isOn && ElectricityMeter.Instance != null)
        {
            ElectricityMeter.Instance.RemoveLoad(powerRating);
        }
    }

    // Tự động bật (Mở cửa) khi người chơi bước vào vùng cảm biến
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && doorHinge != null)
        {
            if (!isOn) ToggleDevice(); 
        }
    }

    // Tự động tắt (Đóng cửa) khi người chơi bước ra khỏi vùng cảm biến
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && doorHinge != null)
        {
            if (isOn) ToggleDevice(); 
        }
    }
}