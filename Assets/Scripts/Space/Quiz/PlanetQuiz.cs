using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class PlanetQuiz : MonoBehaviour
{
    public static PlanetQuiz Instance;

    [Header("UI Toolkit")]

    // Các element lấy từ UXML
    public GameObject quizPanel;
    public TextMeshProUGUI _questionText;
    public GameObject[] buttons;
    public TextMeshProUGUI _feedbackText;
    
    private Color normalColor;

    // [GIỮ NGUYÊN] Complete panel vẫn dùng GameObject thường
    public GameObject completePanel;

    [Header("World Space Settings")]
    [Tooltip("Khoảng cách sang trái so với hành tinh")]
    public float leftOffset = 0.4f;

    [Tooltip("Khoảng cách lên cao so với hành tinh")]
    public float upOffset = 0.1f;

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

        // [UI TOOLKIT] Ẩn UI lúc đầu
        HideUI();

        if (completePanel != null) completePanel.SetActive(false);
        if (_feedbackText != null) _feedbackText.text = "";
        
        InitAllLabels();
    }

    // [UI TOOLKIT] Ẩn toàn bộ UI
    private void HideUI()
    {
        quizPanel.SetActive(false);
    }

    // [UI TOOLKIT] Hiện toàn bộ UI
    private void ShowUI()
    {
       quizPanel.SetActive(true);
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
        if (targetCamera == null ) return;

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
        if (_questionText != null) _questionText.text = "Hành tinh này tên là gì?";

        List<string> options = GetRandomOptions(planetName);
        for (int i = 0; i < buttons.Length; i++)
        {
            SetupButton(buttons[i].GetComponent<Button>(), options[i]);
        }
        
        normalColor = buttons[0].GetComponent<Image>().color;

        foreach (var button in buttons)
        {
            ResetButtonColor(button.GetComponent<Button>());
        }


        PlaceScreenNextToPlanet();

        // [UI TOOLKIT] Hiện UI
        ShowUI();
    }

    private void SetupButton(Button btn, string name)
    {
        if (btn == null) return;
        btn.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = name;

        // [UI TOOLKIT] Xóa listener cũ và gán mới
        btn.onClick.AddListener(() => OnAnswer(btn, name));
    }

    private void OnAnswer(Button btn, string answer)
    {
        if (answer == correctAnswer)
        {
            SetButtonColor(btn, Color.green);
            if (_feedbackText != null)
            {
                _feedbackText.text = "Chính xác!";
                _feedbackText.color = Color.green;
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
                _feedbackText.color = Color.red;
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
            btn.GetComponent<Image>().color = color;
    }

    private void ResetButtonColor(Button btn)
    {
        if (btn != null)
            btn.GetComponent<Image>().color = normalColor;
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