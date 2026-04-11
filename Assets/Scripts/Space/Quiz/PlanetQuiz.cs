using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;

public class PlanetQuiz : MonoBehaviour
{
    public static PlanetQuiz Instance;

    [Header("UI Toolkit")]
    public UIDocument uiDocument; // Kéo QuizUIDocument vào đây

    // Các element lấy từ UXML
    private VisualElement _root;
    private Label _questionText;
    private Button _buttonA;
    private Button _buttonB;
    private Button _buttonC;
    private Button _buttonD;
    private Label _feedbackText;

    // [GIỮ NGUYÊN] Complete panel vẫn dùng GameObject thường
    public GameObject completePanel;

    [Header("World Space Settings")]
    [Tooltip("Khoảng cách sang trái so với hành tinh")]
    public float leftOffset = 0.8f;

    [Tooltip("Khoảng cách lên cao so với hành tinh")]
    public float upOffset = 0.5f;

    public Camera targetCamera;

    // [GIỮ NGUYÊN] Audio feedback
    [Header("Audio Feedback")]
    public AudioSource audioSource;
    public AudioClip correctClip;
    public AudioClip wrongClip;

    private string[] allPlanets = { "Mercury", "Venus", "Earth", "Mars", "Jupiter", "Saturn", "Uranus", "Neptune" };

    private string correctAnswer = "";
    [SerializeField] private HashSet<string> answeredPlanets = new HashSet<string>();

    private PlanetVisual currentPlanetVisual = null;

    private void Awake() => Instance = this;

    private void Start()
    {
        if (targetCamera == null) targetCamera = Camera.main;

        // [UI TOOLKIT] Lấy các element từ UXML
        _root         = uiDocument.rootVisualElement;
        _questionText = _root.Q<Label>("question-text");
        _buttonA      = _root.Q<Button>("button-a");
        _buttonB      = _root.Q<Button>("button-b");
        _buttonC      = _root.Q<Button>("button-c");
        _buttonD      = _root.Q<Button>("button-d");
        _feedbackText = _root.Q<Label>("feedback-text");

        // [UI TOOLKIT] Ẩn UI lúc đầu
        HideUI();

        if (completePanel != null) completePanel.SetActive(false);
        if (_feedbackText != null) _feedbackText.text = "";

        InitAllLabels();
    }

    // [UI TOOLKIT] Ẩn toàn bộ UI
    private void HideUI()
    {
        if (_root != null)
            _root.style.display = DisplayStyle.None;
    }

    // [UI TOOLKIT] Hiện toàn bộ UI
    private void ShowUI()
    {
        if (_root != null)
            _root.style.display = DisplayStyle.Flex;
    }

    private void InitAllLabels()
    {
        PlanetVisual[] allVisuals = FindObjectsOfType<PlanetVisual>();
        foreach (PlanetVisual v in allVisuals)
            v.ShowQuestionMark();
    }

    // [UI TOOLKIT] Đặt vị trí UIDocument cạnh hành tinh
    private void PlaceScreenNextToPlanet()
    {
        if (targetCamera == null || uiDocument == null) return;

        Vector3 planetPos = SolarSystemFocus.Instance.GetFocusWorldPos();

        uiDocument.transform.position = planetPos
            - targetCamera.transform.right * leftOffset
            + targetCamera.transform.up   * upOffset;

        Vector3 dirToCamera = targetCamera.transform.position - uiDocument.transform.position;
        uiDocument.transform.rotation = Quaternion.LookRotation(-dirToCamera.normalized);
    }

    public void StartQuiz(string planetName, PlanetVisual visual)
    {
        if (answeredPlanets.Contains(planetName)) return;

        correctAnswer       = planetName;
        currentPlanetVisual = visual;
        if (_feedbackText != null) _feedbackText.text = "";
        if (_questionText != null) _questionText.text = "Hành tinh này tên là gì?";

        List<string> options = GetRandomOptions(planetName);
        SetupButton(_buttonA, options[0]);
        SetupButton(_buttonB, options[1]);
        SetupButton(_buttonC, options[2]);
        SetupButton(_buttonD, options[3]);

        ResetButtonColor(_buttonA);
        ResetButtonColor(_buttonB);
        ResetButtonColor(_buttonC);
        ResetButtonColor(_buttonD);

        PlaceScreenNextToPlanet();

        // [UI TOOLKIT] Hiện UI
        ShowUI();
    }

    private void SetupButton(Button btn, string name)
    {
        if (btn == null) return;
        btn.text = name;

        // [UI TOOLKIT] Xóa listener cũ và gán mới
        btn.clickable = new Clickable(() => OnAnswer(btn, name));
    }

    private void OnAnswer(Button btn, string answer)
    {
        if (answer == correctAnswer)
        {
            SetButtonColor(btn, Color.green);
            if (_feedbackText != null)
            {
                _feedbackText.text = "Chính xác!";
                _feedbackText.style.color = new StyleColor(Color.green);
            }

            answeredPlanets.Add(correctAnswer);

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
                _feedbackText.text = "Sai rồi! Thử lại nhé.";
                _feedbackText.style.color = new StyleColor(Color.red);
            }

            if (audioSource != null && wrongClip != null)
                audioSource.PlayOneShot(wrongClip);
        }
    }

    private IEnumerator CorrectRoutine()
    {
        yield return new WaitForSeconds(correctClip != null ? correctClip.length : 1.5f);

        // [UI TOOLKIT] Ẩn UI
        HideUI();
        if (_feedbackText != null) _feedbackText.text = "";

        if (AnswerTrigger.Instance != null)
        {
            AnswerTrigger.Instance.PlayDescription(correctAnswer, () =>
            {
                if (SolarSystemFocus.Instance != null)
                    SolarSystemFocus.Instance.ZoomOut();

                if (answeredPlanets.Count >= allPlanets.Length)
                    StartCoroutine(ShowComplete());
            });
        }
        else
        {
            if (SolarSystemFocus.Instance != null)
                SolarSystemFocus.Instance.ZoomOut();

            if (answeredPlanets.Count >= allPlanets.Length)
                StartCoroutine(ShowComplete());
        }
    }

    private IEnumerator ShowComplete()
    {
        yield return new WaitForSeconds(2f);
        if (completePanel != null) completePanel.SetActive(true);
    }

    private void SetButtonColor(Button btn, Color color)
    {
        if (btn != null)
            btn.style.backgroundColor = new StyleColor(color);
    }

    private void ResetButtonColor(Button btn)
    {
        if (btn != null)
            btn.style.backgroundColor = StyleKeyword.Null;
    }

    private List<string> GetRandomOptions(string correct)
    {
        List<string> wrong = new List<string>();
        foreach (string p in allPlanets)
            if (p != correct) wrong.Add(p);

        for (int i = wrong.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            string tmp = wrong[i]; wrong[i] = wrong[j]; wrong[j] = tmp;
        }

        List<string> options = new List<string> { correct, wrong[0], wrong[1], wrong[2] };
        for (int i = options.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            string tmp = options[i]; options[i] = options[j]; options[j] = tmp;
        }

        return options;
    }

    public bool IsAnswered(string planetName)
    {
        return answeredPlanets.Contains(planetName);
    }
}