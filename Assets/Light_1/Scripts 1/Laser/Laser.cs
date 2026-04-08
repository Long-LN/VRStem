
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Windows;

public class Laser : MonoBehaviour
{
    bool activated = false;

    LineRenderer lineRenderer;
    [SerializeField] LaserRendererSettings LaserRendererSettings;

    Vector3 sourcePosition;
    const float farDistance = 1000f;
    List<Vector3> bouncePositions;
    int maxBounces = 100;

    LaserSensor prevStruckLaserSensor = null;

    [SerializeField] GameObject inputGO;

    public IInput input { get; private set; }

    void Awake()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        LaserRendererSettings.Apply(lineRenderer);

        if (inputGO == null)
        {
            BehaviourIfNullInput();
            return;
        }

        input = inputGO.GetComponent<IInput>();

        if (input == null)
        {
            Debug.Log($"The input GameObject attached to {name} must contain a script which ");
            return;
        }

        RegisterToInput(input);

    }

    void FixedUpdate()
    {
        if (!activated)
        {
            lineRenderer.positionCount = 0;
            if (prevStruckLaserSensor != null)
            {
                LaserSensor.HandleLaser(this, prevStruckLaserSensor, null);
                prevStruckLaserSensor = null;
            }
            return;
        }

        sourcePosition = transform.position + transform.forward * 0.2501f;
        bouncePositions = new List<Vector3>() { sourcePosition };

        CastBeam(sourcePosition, transform.forward);

        lineRenderer.positionCount = bouncePositions.Count;
        lineRenderer.SetPositions(bouncePositions.ToArray());
    }

    public void CastBeam(Vector3 origin, Vector3 direction)
    {
        if (bouncePositions.Count > maxBounces)
            return;

        var ray = new Ray(origin, direction);

        bool didHit = Physics.Raycast(ray, out RaycastHit hitInfo, farDistance);

        if (!didHit)
        {
            var endPoint = origin + direction * farDistance;
            bouncePositions.Add(endPoint);
            if (prevStruckLaserSensor != null)
            {
                LaserSensor.HandleLaser(this, prevStruckLaserSensor, null);
                prevStruckLaserSensor = null;
            }
            return;
        }

        bouncePositions.Add(hitInfo.point);

        var reflectiveObject = hitInfo.collider.GetComponent<ILaserReflective>();

        if (reflectiveObject != null)
            reflectiveObject.Reflect(this, ray, hitInfo);

        else
        {
            var currentLaserSensor = hitInfo.collider.GetComponent<LaserSensor>();
            if (currentLaserSensor != prevStruckLaserSensor)
            {
                LaserSensor.HandleLaser(this, prevStruckLaserSensor, currentLaserSensor);
                prevStruckLaserSensor = currentLaserSensor;
            }
        }
    }

    public void RegisterToInput(IInput inputTarget)
    {
        // 1. Đăng ký sự kiện thông qua các phương thức cụ thể để dễ quản lý
        inputTarget.onTriggered += HandleTriggered;
        inputTarget.onUntriggered += HandleUntriggered;

        // 2. Đồng bộ trạng thái ngay lập tức
        // Đảm bảo nếu input (ví dụ: công tắc) đã được kích hoạt từ trước, laser sẽ bật ngay.
        activated = inputTarget.IsTriggered;
    }

    // Các phương thức xử lý sự kiện
    private void HandleTriggered(IInput input)
    {
        activated = true;
    }

    private void HandleUntriggered(IInput input)
    {
        activated = false;
    }

    // BẮT BUỘC: Hủy đăng ký sự kiện khi script hoặc object chứa tia laser bị hủy
    private void OnDestroy()
    {
        if (input != null)
        {
            input.onTriggered -= HandleTriggered;
            input.onUntriggered -= HandleUntriggered;
        }
    }

    // Bổ sung hàm bạn còn gọi thiếu trong Awake() của file Laser.cs
    private void BehaviourIfNullInput()
    {
        // Nếu không có nguồn điều khiển nào được gán, mặc định tia laser sẽ luôn bật
        activated = true;
    }
}
