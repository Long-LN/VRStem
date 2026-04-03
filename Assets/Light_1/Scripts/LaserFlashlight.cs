using UnityEngine;

public class LaserFlashlight : MonoBehaviour
{
    [Header("Laser Settings")]
    public Transform sourceTransform;
    public LaserBeam laserBeam;

    [Header("Flashlight Effects")]
    public Light pointLight;
    public Material lensMaterial;
    //public AudioSource audioSource;

    private bool isOn = false;

    private void Start()
    {
        // Đảm bảo mọi thứ tắt khi mới vào game
        TurnOff();
    }

    void Update()
    {
        // Nếu đang tắt thì không làm gì cả để tiết kiệm hiệu năng
        if (!isOn) return;

        // Cập nhật vị trí và hướng bắn của laser liên tục khi đang bật
        Vector3 startPosition = sourceTransform.position;
        Vector3 direction = sourceTransform.forward;
        laserBeam.Propagate(startPosition, direction);
    }

    // Hàm gọi khi cầm đèn lên hoặc bóp cò tay cầm VR
    public void TurnOn()
    {
        if (isOn) return;
        isOn = true;

        // Bật Laser
        if (laserBeam != null) laserBeam.gameObject.SetActive(true);

        // Bật hiệu ứng đèn pin
        if (pointLight != null) pointLight.enabled = true;
        if (lensMaterial != null) lensMaterial.EnableKeyword("_EMISSION");
        //if (audioSource != null) audioSource.Play();
    }

    // Hàm gọi khi bỏ đèn xuống hoặc nhả cò
    public void TurnOff()
    {
        if (!isOn) return;
        isOn = false;

        // Tắt Laser (xoá tia)
        if (laserBeam != null) laserBeam.gameObject.SetActive(false);

        // Tắt hiệu ứng đèn pin
        if (pointLight != null) pointLight.enabled = false;
        if (lensMaterial != null) lensMaterial.DisableKeyword("_EMISSION");
        //if (audioSource != null) audioSource.Play(); // Phát tiếng "tách" khi tắt
    }

    // Nút gạt bật/tắt luân phiên
    public void ToggleLight()
    {
        if (isOn) TurnOff();
        else TurnOn();
    }
}