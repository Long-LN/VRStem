using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Handle : MonoBehaviour
{
    [Header("Movement Points")]
    public Transform startPoint; // Kéo GameObject rỗng đầu vào đây
    public Transform endPoint;   // Kéo GameObject rỗng cuối vào đây
    
    // private Transform handle;
    // private Transform parent;
    private Rigidbody rb;
    private bool isGrabbed = false;
    XRGrabInteractable grab;
    
    public UnityEvent<float> OnProgressChanged;
    
    private float _lastProgress = -1f;
    public float progress;
    
    void Start()
    {
        // parent = transform.parent;
        // handle = GetComponent<Transform>();
        grab = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();
        grab.selectEntered.AddListener(_ => isGrabbed = true);
        grab.selectExited.AddListener(_ => isGrabbed = false);
        grab.selectExited.AddListener(OnRelease);
        
        UpdateHandleByScale(SolarSystemFocus.Instance.minScale);
    }
    
    
    void OnRelease(SelectExitEventArgs args)
    {
        // StartCoroutine(StopNextFrame());
        
        // rb.linearVelocity = Vector3.zero;
        // rb.angularVelocity = Vector3.zero;
        // rb.Sleep();
    }

    IEnumerator StopNextFrame()
    {
        yield return null;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.Sleep();
    }

    void Update()
    {
        // // vector từ parent đến object
        // Vector3 dir = transform.position - parent.position;
        //
        // // lấy trục X của parent (local X)
        // Vector3 axis = parent.forward;
        //
        // // chiếu vector dir lên axis
        // float distance = Vector3.Dot(dir, axis);
        //
        // // set lại position chỉ theo trục X của parent
        // transform.position = parent.position + axis * distance;
        //
        // if (!isGrabbed) return;
        // progress = (handle.position.x - initPos) / maxX * 100f;
        // Debug.Log(progress);
    }

    private void LateUpdate()
    {
        // 1. Chuyển vị trí hiện tại của handle sang không gian local của startPoint
        Vector3 localPos = startPoint.InverseTransformPoint(transform.position);

        // 2. Tính toán hướng và khoảng cách tối đa giữa 2 điểm
        Vector3 targetDirection = startPoint.InverseTransformPoint(endPoint.position);
        float maxDistance = targetDirection.magnitude;

        // 3. Chiếu vị trí hiện tại lên trục nối 2 điểm và Clamp (Giới hạn)
        // Chúng ta chỉ cho phép di chuyển trên trục Z local của hướng nối 2 điểm
        float dot = Vector3.Dot(localPos, targetDirection.normalized);
        dot = Mathf.Clamp(dot, 0, maxDistance);

        // 4. Áp dụng vị trí đã giới hạn (World Space)
        transform.position = startPoint.TransformPoint(targetDirection.normalized * dot);

        // 5. Tính Progress (0 - 100)
        progress = (dot / maxDistance) * 100f;
        float newProgress = (dot / maxDistance) * 100f;

        // KIỂM TRA THAY ĐỔI: Chỉ kích hoạt sự kiện nếu giá trị thực sự thay đổi
        if (!Mathf.Approximately(newProgress, _lastProgress))
        {
            progress = newProgress;
            _lastProgress = newProgress;
            
            // Kích hoạt sự kiện và gửi giá trị progress đi
            // OnProgressChanged?.Invoke(progress);
            SolarSystemFocus.Instance.SetSystemScale(progress);
            if (isGrabbed)
            {
                Debug.Log("Interupt");
                SolarSystemFocus.Instance.focusIn = false;
                SolarSystemFocus.Instance.focusOut = false;
            }
        }
    }
    
    public void UpdateHandleByScale(float currentSystemScale)
    {
        // Nếu người dùng đang cầm nắm thì không cho hệ thống tự động di chuyển Handle
        if (isGrabbed || startPoint == null || endPoint == null) return;

        // 1. Tính toán ngược lại Progress dựa trên Scale hiện tại (Inverse Lerp)
        // Giả sử bạn dùng lại minSystemScale và maxSystemScale giống bên SolarSystemFocus
        float minSystemScale = SolarSystemFocus.Instance.minScale; // Nên để biến public để khớp với SolarSystemFocus
        float maxSystemScale = SolarSystemFocus.Instance.targetScale; 

        float t = Mathf.InverseLerp(minSystemScale, maxSystemScale, currentSystemScale);
        progress = t * 100f;

        // 2. Cập nhật vị trí Handle theo đường thẳng nối 2 điểm
        Vector3 targetDir = endPoint.position - startPoint.position;
        transform.position = startPoint.position + targetDir * t;
        
    }
}
