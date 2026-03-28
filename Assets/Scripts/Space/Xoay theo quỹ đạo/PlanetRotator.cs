using UnityEngine;
using UnityEngine.InputSystem;

public class PlanetRotator : MonoBehaviour
{
    public static PlanetRotator Instance;

    [Header("Rotation Settings")]
    public float rotationSpeed = 5f;

    private GameObject currentPlanet;
    private bool isRotating = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        // Chỉ xoay khi có hành tinh được set
        if (currentPlanet == null)
        {
            Debug.Log("[PlanetRotator] currentPlanet is NULL");
            return;
        }

        if (Keyboard.current.gKey.isPressed)
        {
            isRotating = true;

            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            Debug.Log($"[PlanetRotator] Rotating {currentPlanet.name}, mouseDelta: {mouseDelta}");

            float mouseX = mouseDelta.x;
            float mouseY = mouseDelta.y;

            currentPlanet.transform.Rotate(Vector3.up, -mouseX * rotationSpeed, Space.World);
            currentPlanet.transform.Rotate(Vector3.right, mouseY * rotationSpeed, Space.World);
        }
        else
        {
            isRotating = false;
        }
    }

    public void SetPlanet(GameObject planet)
    {
        Debug.Log($"[PlanetRotator] SetPlanet: {planet?.name}");
        currentPlanet = planet;
    }

    public void ClearPlanet()
    {
        currentPlanet = null;
        isRotating = false;
    }
}