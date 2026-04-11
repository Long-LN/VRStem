using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.UIElements;

// Alias để tránh xung đột giữa UnityEngine.UI và UnityEngine.UIElements
using UIButton = UnityEngine.UI.Button;
using UILabel = UnityEngine.UIElements.Label;
using UIImage = UnityEngine.UI.Image;

public class PlanetQuiz : MonoBehaviour
{
    public static PlanetQuiz Instance;

    [Header("UI Toolkit")]
    public GameObject quizPanel;
    public TextMeshProUGUI _questionText;
    public GameObject[] buttons;
    public TextMeshProUGUI _feedbackText;

    private Color normalColor;

    public GameObject completePanel;

    [Header("World Space Settings")]
    [Tooltip("Khoảng cách sang trái so với hành tinh")]
    public float leftOffset = 0.4f;

    [Tooltip("Khoảng cách lên cao so với hành tinh")]
    public float upOffset = 0.1f;

    public Camera targetCamera;

    [Header("Audio Feedback")]
    public AudioSource audioSource;
    public AudioClip correctClip;
    public AudioClip wrongClip;

    private string[] allPlanets = { "Mercury", "Venus", "Earth", "Mars", "Jupiter", "Saturn", "Uranus", "Neptune" };

    private string correctAnswer = "";
    [SerializeField] private HashSet<string> answeredPlanets = new HashSet<string>();

    private PlanetVisual currentPlanetVisual = null;

    // ─────────────────────────────────────────────
    // Description Panel (UI Toolkit)
    // ─────────────────────────────────────────────
    [Header("Description Panel")]
    public UIDocument descriptionUIDocument;

    private VisualElement _descRoot;
    private UILabel _planetNameLabel;
    private UILabel _descriptionTextLabel;

    private void Awake() => Instance = this;

    private void Start()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        HideUI();

        if (completePanel != null)
            completePanel.SetActive(false);
        else
            Debug.LogWarning("[PlanetQuiz] completePanel is NULL! Assign it in Inspector.");

        if (_feedbackText != null)
            _feedbackText.text = "";
        else
            Debug.LogWarning("[PlanetQuiz] _feedbackText is NULL! Assign it in Inspector.");

        // Ẩn description panel ngay từ đầu
        if (descriptionUIDocument != null)
        {
            descriptionUIDocument.gameObject.SetActive(false);
            var root = descriptionUIDocument.rootVisualElement;
            if (root != null)
                root.style.display = DisplayStyle.None;
        }
        else
        {
            Debug.LogError("[PlanetQuiz] descriptionUIDocument is NULL! Kéo UIDocument vào Inspector.");
        }

        InitAllLabels();
    }

    private void HideUI() => quizPanel.SetActive(false);

    private void ShowUI() => quizPanel.SetActive(true);

    private void InitAllLabels()
    {
        PlanetVisual[] allVisuals = FindObjectsOfType<PlanetVisual>();
        foreach (PlanetVisual v in allVisuals)
            v.ShowQuestionMark();
    }

    private void PlaceScreenNextToPlanet()
    {
        if (targetCamera == null) return;
        if (SolarSystemFocus.Instance == null || SolarSystemFocus.Instance.pivot == null)
        {
            Debug.LogError("[PlanetQuiz] SolarSystemFocus.Instance hoặc pivot là NULL!");
            return;
        }

        Vector3 planetPos = SolarSystemFocus.Instance.pivot.position;

        quizPanel.transform.position = planetPos
            - targetCamera.transform.right * leftOffset
            + targetCamera.transform.up   * upOffset;

        Vector3 dirToCamera = targetCamera.transform.position - quizPanel.transform.position;
        quizPanel.transform.rotation = Quaternion.LookRotation(-dirToCamera.normalized);
    }

    public void StartQuiz(string planetName, PlanetVisual visual)
    {
        if (answeredPlanets.Contains(planetName)) return;

        correctAnswer       = planetName;
        currentPlanetVisual = visual;

        if (_feedbackText != null) _feedbackText.text = "";
        if (_questionText != null)
            _questionText.text = "Hành tinh này tên là gì?";

        List<string> options = GetRandomOptions(planetName);

        if (buttons == null || buttons.Length == 0)
        {
            Debug.LogError("[PlanetQuiz] buttons array is NULL hoặc rỗng!");
            return;
        }

        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] == null) continue;
            UIButton btn = buttons[i].GetComponent<UIButton>();
            if (btn == null) continue;
            SetupButton(btn, options[i]);
        }

        UIImage firstImg = buttons[0].GetComponent<UIImage>();
        if (firstImg != null)
            normalColor = firstImg.color;

        foreach (var button in buttons)
        {
            if (button != null)
                ResetButtonColor(button.GetComponent<UIButton>());
        }

        PlaceScreenNextToPlanet();
        ShowUI();
    }

    private void SetupButton(UIButton btn, string name)
    {
        if (btn == null) return;

        btn.onClick.RemoveAllListeners();

        Transform labelChild = btn.transform.GetChild(1);
        if (labelChild == null)
        {
            Debug.LogError("[PlanetQuiz] SetupButton() → không tìm thấy child[1] của button!");
            return;
        }

        TextMeshProUGUI tmp = labelChild.GetComponent<TextMeshProUGUI>();
        if (tmp == null)
        {
            Debug.LogError("[PlanetQuiz] SetupButton() → child[1] không có TextMeshProUGUI!");
            return;
        }

        tmp.text = name;
        btn.onClick.AddListener(() => OnAnswer(btn, name));
    }

    private void OnAnswer(UIButton btn, string answer)
    {
        if (answer == correctAnswer)
        {
            SetButtonColor(btn, Color.green);

            if (_feedbackText != null)
            {
                _feedbackText.text  = "Chính xác!";
                _feedbackText.color = Color.green;
            }

            answeredPlanets.Add(correctAnswer);
            Debug.Log($"[PlanetQuiz] Đúng! {correctAnswer} — đã trả lời {answeredPlanets.Count}/{allPlanets.Length}");

            if (currentPlanetVisual != null)
                currentPlanetVisual.ShowCorrectLabel();

            if (audioSource != null && correctClip != null)
                audioSource.PlayOneShot(correctClip);

            StartCoroutine(CorrectRoutine());
        }
        else
        {
            SetButtonColor(btn, Color.red);

            if (_feedbackText != null)
            {
                _feedbackText.text  = "Sai rồi! Thử lại nhé.";
                _feedbackText.color = Color.red;
            }

            if (audioSource != null && wrongClip != null)
                audioSource.PlayOneShot(wrongClip);
        }
    }

    private IEnumerator CorrectRoutine()
    {
        float waitTime = correctClip != null ? correctClip.length : 1.5f;
        yield return new WaitForSeconds(waitTime);

        HideUI();
        if (_feedbackText != null) _feedbackText.text = "";

        if (AnswerTrigger.Instance != null)
        {
            ShowDescription(correctAnswer);

            AnswerTrigger.Instance.PlayDescription(correctAnswer, () =>
            {
                HideDescription();

                if (SolarSystemFocus.Instance != null)
                    SolarSystemFocus.Instance.ZoomOut();
                else
                    Debug.LogWarning("[PlanetQuiz] SolarSystemFocus.Instance is NULL trong callback!");

                if (answeredPlanets.Count >= allPlanets.Length)
                    StartCoroutine(ShowComplete());
            });
        }
        else
        {
            Debug.LogWarning("[PlanetQuiz] AnswerTrigger.Instance is NULL!");

            if (SolarSystemFocus.Instance != null)
                SolarSystemFocus.Instance.ZoomOut();

            if (answeredPlanets.Count >= allPlanets.Length)
                StartCoroutine(ShowComplete());
        }
    }

    private IEnumerator ShowComplete()
    {
        yield return new WaitForSeconds(2f);

        if (completePanel != null)
            completePanel.SetActive(true);
        else
            Debug.LogError("[PlanetQuiz] ShowComplete() → completePanel is NULL!");
    }

    private void SetButtonColor(UIButton btn, Color color)
    {
        UIImage img = btn?.GetComponent<UIImage>();
        if (img != null) img.color = color;
    }

    private void ResetButtonColor(UIButton btn)
    {
        UIImage img = btn?.GetComponent<UIImage>();
        if (img != null) img.color = normalColor;
    }

    private List<string> GetRandomOptions(string correct)
    {
        List<string> wrong = new List<string>();
        foreach (string p in allPlanets)
            if (p != correct) wrong.Add(p);

        for (int i = wrong.Count - 1; i > 0; i--)
        {
            int j      = Random.Range(0, i + 1);
            string tmp = wrong[i]; wrong[i] = wrong[j]; wrong[j] = tmp;
        }

        List<string> options = new List<string> { correct, wrong[0], wrong[1], wrong[2] };
        for (int i = options.Count - 1; i > 0; i--)
        {
            int j      = Random.Range(0, i + 1);
            string tmp = options[i]; options[i] = options[j]; options[j] = tmp;
        }

        return options;
    }

    public bool IsAnswered(string planetName) => answeredPlanets.Contains(planetName);

    // ─────────────────────────────────────────────
    // Các hàm xử lý Description Panel
    // ─────────────────────────────────────────────

    private void ShowDescription(string planetName)
    {
        if (descriptionUIDocument == null)
        {
            Debug.LogError("[PlanetQuiz] descriptionUIDocument là NULL!");
            return;
        }

        descriptionUIDocument.gameObject.SetActive(true);

        _descRoot = descriptionUIDocument.rootVisualElement;
        if (_descRoot == null)
        {
            Debug.LogError("[PlanetQuiz] rootVisualElement là NULL sau SetActive(true)!");
            return;
        }

        _descRoot.style.display = DisplayStyle.Flex;

        _planetNameLabel      = _descRoot.Q<UILabel>("planet-name");
        _descriptionTextLabel = _descRoot.Q<UILabel>("description-text");

        if (_planetNameLabel == null)
            Debug.LogError("[PlanetQuiz] Không tìm thấy Label 'planet-name' trong UXML!");
        if (_descriptionTextLabel == null)
            Debug.LogError("[PlanetQuiz] Không tìm thấy Label 'description-text' trong UXML!");

        PlanetVisual[] allVisuals = FindObjectsOfType<PlanetVisual>();
        PlanetVisual target = System.Array.Find(allVisuals, v => v.planetName == planetName);

        if (target == null)
            Debug.LogWarning($"[PlanetQuiz] Không tìm thấy PlanetVisual cho '{planetName}'!");

        if (_planetNameLabel != null)
            _planetNameLabel.text = planetName;

        if (_descriptionTextLabel != null)
            _descriptionTextLabel.text = target != null ? target.description : "(Không có mô tả)";

        PlaceDescriptionNextToPlanet();
    }

    private void HideDescription()
    {
        if (descriptionUIDocument == null) return;

        var root = descriptionUIDocument.rootVisualElement;
        if (root != null)
            root.style.display = DisplayStyle.None;

        descriptionUIDocument.gameObject.SetActive(false);
    }

    private void PlaceDescriptionNextToPlanet()
    {
        if (targetCamera == null || descriptionUIDocument == null) return;

        if (SolarSystemFocus.Instance == null || SolarSystemFocus.Instance.pivot == null)
        {
            Debug.LogError("[PlanetQuiz] SolarSystemFocus.Instance hoặc pivot là NULL!");
            return;
        }

        Vector3 planetPos = SolarSystemFocus.Instance.pivot.position;

        descriptionUIDocument.transform.position = planetPos
            - targetCamera.transform.right * leftOffset
            + targetCamera.transform.up   * upOffset;

        Vector3 dirToCamera = targetCamera.transform.position - descriptionUIDocument.transform.position;
        descriptionUIDocument.transform.rotation = Quaternion.LookRotation(-dirToCamera.normalized);
    }
}