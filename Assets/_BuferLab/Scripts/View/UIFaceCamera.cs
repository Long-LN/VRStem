using UnityEngine;

public class UIFaceCamera : MonoBehaviour
{
    private Transform mainCamera;

    void Start()
    {
        // Tu dong tim camera chinh (mat cua nguoi choi VR)
        if (Camera.main != null)
        {
            mainCamera = Camera.main.transform;
        }
    }

    // Dung LateUpdate de dam bao UI xoay sau khi moi chuyen dong khac da hoan tat
    void LateUpdate()
    {
        if (mainCamera != null)
        {
            // Phep toan giup mat phang cua UI luon song song voi mat phang cua Camera
            transform.LookAt(transform.position + mainCamera.rotation * Vector3.forward, mainCamera.rotation * Vector3.up);
        }
    }
}