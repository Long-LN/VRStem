using UnityEngine;

public class TrashCanZone : MonoBehaviour
{
    // Hàm này tự động chạy khi có một vật thể (có Rigidbody) chạm vào vùng Trigger này
    private void OnTriggerEnter(Collider other)
    {
        // Kiểm tra xem vật thể rơi vào có phải là Thiết bị thông minh không
        // Dùng GetComponentInParent vì có thể chạm trúng phần cánh quạt hoặc vỏ đèn
        SmartDeviceController device = other.GetComponentInParent<SmartDeviceController>();
        
        if (device != null)
        {
            // BƯỚC 1: Đề phòng trường hợp lỗi (quạt vẫn đang cắm trên tường mà bị vứt đi)
            // Ta cứ báo cho Hub xóa UI trước cho chắc chắn
            if (SmartHomeHub.Instance != null)
            {
                SmartHomeHub.Instance.RemoveDeviceFromUI(device);
            }

            // BƯỚC 2: Tiêu hủy vật thể 3D vĩnh viễn khỏi bộ nhớ
            Destroy(device.gameObject);
            
            Debug.Log("Đã tiêu hủy rác: " + device.gameObject.name);
            
            // (Tùy chọn nâng cao): Bạn có thể thêm một hiệu ứng âm thanh "xoảng" 
            // hoặc hiệu ứng hạt Particle Effect nổ bùm ở đây cho sinh động!
        }
    }
}