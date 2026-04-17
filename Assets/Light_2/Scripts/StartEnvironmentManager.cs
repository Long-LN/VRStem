using UnityEngine;

public class StartEnvironmentManager : MonoBehaviour
{
    public static bool shouldAutoStart = false;

    public Light sun;
    public Material skyboxDayMaterial;
    public Material skyboxNightMaterial;
    public GameObject menuBoard;
    public GameObject plane;
    public GameObject hub;
    public GameObject startButton;
    public SequenceSoundManager sequenceSoundManager;

    private void Start()
    {
        if (shouldAutoStart)
        {
            StartEnvironment();
            shouldAutoStart = false; 
            return; 
        }
        // 1. Chỉnh môi trường về màu đen tuyệt đối
        RenderSettings.skybox = skyboxNightMaterial; // Bỏ skybox

        // 2. Tắt ánh sáng mặt trời
        if (sun != null)
            sun.intensity = 0f;

        // 3. Ẩn các GameObject
        if (menuBoard != null)
            menuBoard.SetActive(false);
        if (hub != null)
            hub.SetActive(false);

        // 4. Ẩn Plane nhưng giữ va chạm
        if (plane != null && plane.GetComponent<MeshRenderer>() != null)
        {
            plane.GetComponent<MeshRenderer>().enabled = false;
        }
        // 5. Phát âm thanh
        if (shouldAutoStart)
        {
            StartEnvironment();

            if (sequenceSoundManager != null)
                sequenceSoundManager.StartFromStep(1);

            shouldAutoStart = false;
            return;
        }
    }

    public void StartEnvironment()
    {
        // 1. Bật lại bầu trời và ánh sáng môi trường
        if (skyboxDayMaterial != null)
        {
            RenderSettings.skybox = skyboxDayMaterial;
        }

        // 2. Bật ánh sáng mặt trời
        if (sun != null)
            sun.intensity = 1f;

        // 3. Hiện các đối tượng và Plane
        if (menuBoard != null)
            menuBoard.SetActive(true);
        if (hub != null)
            hub.SetActive(true);
        if (plane != null && plane.GetComponent<MeshRenderer>() != null)
        {
            plane.GetComponent<MeshRenderer>().enabled = true;
        }

        // 4. Cập nhật lại toàn bộ ánh sáng cảnh vật
        DynamicGI.UpdateEnvironment();

        // 5. Phát sound cho nút Start
        if (sequenceSoundManager != null)
        sequenceSoundManager.TriggerStartButton();
        

        // 6. Ẩn nút Start
        if (startButton != null)
            startButton.SetActive(false);
    }
}
