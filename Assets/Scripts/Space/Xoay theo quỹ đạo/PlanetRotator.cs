using UnityEngine;
using UnityEngine.InputSystem;

public class PlanetRotator : MonoBehaviour
{
    public static PlanetRotator Instance;

    [Header("PC Rotation (G + Mouse)")]
    public float rotationSpeed = 0.2f;

    [Header("VR Rotation (Thumbstick)")]
    public float vrRotationSpeed = 45f;

    private GameObject currentPlanet;

    private InputAction leftThumbstickAction;
    private InputAction rightThumbstickAction;

    private void Awake()
    {
        Instance = this;
        SetupVRInput();
    }

    private void SetupVRInput()
    {
        leftThumbstickAction = new InputAction(
            name: "LeftThumbstick",
            binding: "<XRController>{LeftHand}/thumbstick"
        );
        rightThumbstickAction = new InputAction(
            name: "RightThumbstick",
            binding: "<XRController>{RightHand}/thumbstick"
        );

        leftThumbstickAction.Enable();
        rightThumbstickAction.Enable();
    }

    private void OnDestroy()
    {
        leftThumbstickAction?.Disable();
        rightThumbstickAction?.Disable();
    }

    private void Update()
    {
        if (currentPlanet == null) return;

        HandlePCRotation();
        HandleVRRotation();
    }

    private void HandlePCRotation()
    {
        if (Keyboard.current == null || Mouse.current == null) return;
        if (!Keyboard.current.gKey.isPressed) return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        ApplyRotation(-mouseDelta.x, mouseDelta.y, rotationSpeed);
    }

    private void HandleVRRotation()
    {
        Vector2 input = Vector2.zero;

        if (leftThumbstickAction != null)
            input += leftThumbstickAction.ReadValue<Vector2>();
        if (rightThumbstickAction != null)
            input += rightThumbstickAction.ReadValue<Vector2>();

        if (input.sqrMagnitude < 0.01f) return;

        ApplyRotation(-input.x, input.y, vrRotationSpeed * Time.deltaTime);
    }

    private void ApplyRotation(float inputX, float inputY, float speed)
    {
        if (currentPlanet == null) return;

        Transform camTransform = Camera.main.transform;

        // --- BƯỚC 1: BỨT TẠM INFOPANEL RA KHỎI HÀNH TINH ---
        Transform infoPanel = null;
        Transform originalParent = null;
        
        PlanetVisual pv = currentPlanet.GetComponent<PlanetVisual>();
        if (pv == null) pv = currentPlanet.GetComponentInParent<PlanetVisual>();

        if (pv != null && pv.infoPanel != null)
        {
            infoPanel = pv.infoPanel.transform;
            originalParent = infoPanel.parent;
            
            // SetParent(null) để bứt cái bảng ra không gian ngoài, 
            // đảm bảo nó ĐỨNG IM TUYỆT ĐỐI không bị ảnh hưởng bởi phép xoay
            infoPanel.SetParent(null, true); 
        }

        // --- BƯỚC 2: TÌM TÂM THẬT SỰ CỦA QUẢ CẦU ---
        Renderer[] renderers = currentPlanet.GetComponentsInChildren<Renderer>();
        Bounds bounds = new Bounds();
        bool boundsInitialized = false;

        foreach (Renderer r in renderers)
        {
            // Bỏ qua nếu là Renderer của InfoPanel (đề phòng)
            if (infoPanel != null && r.transform.IsChildOf(infoPanel)) continue;

            if (!boundsInitialized)
            {
                bounds = r.bounds;
                boundsInitialized = true;
            }
            else
            {
                bounds.Encapsulate(r.bounds);
            }
        }

        if (!boundsInitialized) 
        {
            // Nếu không tìm thấy renderer, gắn lại InfoPanel rồi thoát
            if (infoPanel != null) infoPanel.SetParent(originalParent, true);
            return;
        }
        
        Vector3 visualCenter = bounds.center;

        // --- BƯỚC 3: XOAY HÀNH TINH ---
        currentPlanet.transform.RotateAround(visualCenter, Vector3.up, inputX * speed);
        currentPlanet.transform.RotateAround(visualCenter, camTransform.right, inputY * speed);

        // --- BƯỚC 4: LẮP LẠI INFOPANEL VÀO HÀNH TINH ---
        // Lắp lại để khi người dùng Zoom out (kéo cần gạt), cái bảng vẫn thu nhỏ theo hành tinh
        if (infoPanel != null)
        {
            infoPanel.SetParent(originalParent, true);
        }
    }

    public void SetPlanet(GameObject planet)
    {
        currentPlanet = planet;
    }

    public void ClearPlanet()
    {
        currentPlanet = null;
    }
}