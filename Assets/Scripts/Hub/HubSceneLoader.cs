using UnityEngine;
using UnityEngine.SceneManagement;

public class HubSceneLoader : MonoBehaviour
{
    // Hàm này sẽ được gọi khi bấm nút
    public void LoadLesson(string sceneName)
    {
        Debug.Log("Đang dịch chuyển đến: " + sceneName);
        SceneManager.LoadScene(sceneName);
    }

    // Hàm tiện ích để thoát game
    public void QuitGame()
    {
        Debug.Log("Thoát game!");
        Application.Quit();
    }
}