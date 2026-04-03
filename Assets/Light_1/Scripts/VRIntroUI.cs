using UnityEngine;

public class VRIntroUI : MonoBehaviour
{
    public GameObject introPanel;
    public GameObject guidePanel;

    public float autoNextTime = 3f;

    void Start()
    {
        introPanel.SetActive(true);
        guidePanel.SetActive(false);

        Invoke("ShowGuide", autoNextTime);
    }

    void ShowGuide()
    {
        introPanel.SetActive(false);
        guidePanel.SetActive(true);
    }

    public void StartExperience()
    {
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (Camera.main != null)
        {
            Transform cam = Camera.main.transform;

            // Đặt UI trước mặt
            transform.position = cam.position + cam.forward * 5f;

            // Quay về phía người chơi
            transform.LookAt(cam);
            transform.Rotate(0, 180, 0);
        }
    }
}