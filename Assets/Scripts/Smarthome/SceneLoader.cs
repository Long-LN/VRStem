using UnityEngine;
using UnityEngine.SceneManagement; // Bắt buộc phải khai báo thư viện này

public class SceneChanger : MonoBehaviour
{
    public void LoadSmarthome()
    {
        SceneManager.LoadScene("Smart home");
    }
    public void LoadBuffers()
    {
        SceneManager.LoadScene("Buffers");
    }
    public void LoadSpace()
    {
        SceneManager.LoadScene("Space");
    }
}