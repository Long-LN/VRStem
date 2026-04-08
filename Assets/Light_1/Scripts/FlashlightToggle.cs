using UnityEngine;

public class FlashlightToggle : MonoBehaviour
{
    public Light flashlight;
    private bool isOn = false;

    public void ToggleLight()
    {
        isOn = !isOn;
        flashlight.enabled = isOn;
    }
}