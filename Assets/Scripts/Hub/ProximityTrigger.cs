using UnityEngine;

public class ProximityTrigger : MonoBehaviour
{
    [Header("AI Controller")]
    public HubAIGuide hubAI;

    [Header("Settings")]
    public float activationDistance = 3.0f; // Khoảng cách kích hoạt (Đơn vị: mét)

    private bool hasTriggered = false; // Biến nhớ để AI chỉ chào 1 lần duy nhất
    private Transform playerCamera;

    void Start()
    {
        // Tự động tìm cái kính VR của người chơi (Camera mặc định)
        if (Camera.main != null)
        {
            playerCamera = Camera.main.transform;
        }
        else
        {
            Debug.LogError("Không tìm thấy Camera.main! Hãy chắc chắn Main Camera của XR Origin có gắn tag 'MainCamera'.");
        }
    }

    void Update()
    {
        // Nếu AI đã chào rồi, hoặc không tìm thấy Camera, hoặc chưa gắn AI thì bỏ qua không làm gì cả
        if (hasTriggered || playerCamera == null || hubAI == null) return;

        // Tính toán khoảng cách từ vị trí cục Radar này đến đầu người chơi
        float distance = Vector3.Distance(transform.position, playerCamera.position);

        // Nếu người chơi bước vào vùng (khoảng cách nhỏ hơn hoặc bằng 3 mét)
        if (distance <= activationDistance)
        {
            hasTriggered = true; // Đánh dấu là đã kích hoạt để không bị lặp lại
            Debug.Log("Người chơi đã vào vùng menu. Kích hoạt AI!");

            // Gọi AI bắt đầu nói phần 1
            hubAI.StartIntroPart1();
        }
    }

    // Hàm vẽ vòng tròn tàng hình để bạn dễ căn chỉnh khoảng cách trong Scene
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.3f); // Vòng tròn màu xanh lá mờ
        Gizmos.DrawWireSphere(transform.position, activationDistance);
    }
}