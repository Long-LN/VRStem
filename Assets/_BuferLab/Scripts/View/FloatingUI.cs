using TMPro; // Thu vien TextMeshPro de hien thi chu
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit; // Thu vien tuong tac VR
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class FloatingUI : MonoBehaviour
{
    [Header("Ket noi Du lieu")]
    [Tooltip("Keo file LiquidContainer cua cai coc vao day")]
    public LiquidContainer myContainer;

    [Tooltip("Keo code XR Grab Interactable cua cai coc vao day")]
    public XRGrabInteractable grabInteractable;

    [Header("Ket noi Giao dien (UI)")]
    [Tooltip("Keo object Canvas vao day de bat/tat")]
    public GameObject uiCanvas;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI volumeText;
    public TextMeshProUGUI phText;

    // Ham OnEnable va OnDisable giup lang nghe su kien bat/nhay cua tay VR
    void OnEnable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrab);
            grabInteractable.selectExited.AddListener(OnRelease);
        }
    }

    void OnDisable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrab);
            grabInteractable.selectExited.RemoveListener(OnRelease);
        }
    }

    void Start()
    {
        // Tat UI mac dinh khi bat dau game de tiet kiem hieu nang
        if (uiCanvas != null)
        {
            uiCanvas.SetActive(false);
        }
    }

    void Update()
    {
        // Chi cap nhat con so (tinh toan) neu UI dang duoc bat (coc dang cam tren tay)
        if (uiCanvas != null && uiCanvas.activeSelf && myContainer != null)
        {
            UpdateUI();
        }
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        // Bat Canvas len khi ban tay VR nam vao coc
        if (uiCanvas != null)
        {
            uiCanvas.SetActive(true);
            UpdateUI();
        }
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        // Tat Canvas di khi ban tay buong ra
        if (uiCanvas != null)
        {
            uiCanvas.SetActive(false);
        }
    }

    // Ham lay du lieu tu tang Model va in ra man hinh
    private void UpdateUI()
    {
        LiquidData data = myContainer.liquidData;

        if (nameText != null) nameText.text = data.liquidName;

        // Hien thi the tich voi 1 chu so thap phan (F1)
        if (volumeText != null) volumeText.text = "The tich: " + data.volume.ToString("F1") + " ml";

        // Hien thi pH voi 2 chu so thap phan (F2)
        if (phText != null) phText.text = "pH: " + data.phValue.ToString("F2");
    }
}