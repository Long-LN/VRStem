using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class PlanetQuiz : MonoBehaviour
{
    public static PlanetQuiz Instance;

    [Header("UI References")]
    public GameObject quizPanel;
    public TextMeshProUGUI questionText;
    public Button buttonA;
    public Button buttonB;
    public Button buttonC;
    public Button buttonD;
    public TextMeshProUGUI feedbackText;
    public GameObject completePanel;

    [Header("World Space Settings")]
    public Canvas quizCanvas;
    public float heightAbovePlanet = 2f; // Độ cao phía trên hành tinh
    public Camera targetCamera;

    private string[] allPlanets = { "Mercury", "Venus", "Earth", "Mars", "Jupiter", "Saturn", "Uranus", "Neptune" };

    private string correctAnswer = "";
    private HashSet<string> answeredPlanets = new HashSet<string>();

    private PlanetVisual currentPlanetVisual = null;

    private void Awake() => Instance = this;

    private void Start()
    {
        quizPanel.SetActive(false);
        completePanel.SetActive(false);
        feedbackText.text = "";

        InitAllLabels();
    }

    private void InitAllLabels()
    {
        PlanetVisual[] allVisuals = FindObjectsOfType<PlanetVisual>();
        foreach (PlanetVisual v in allVisuals)
            v.ShowQuestionMark();
    }

    private void PlaceCanvasAbovePlanet()
    {
        if (targetCamera == null || quizCanvas == null) return;
        if (currentPlanetVisual == null) return;

        // Đặt canvas phía trên đầu hành tinh
        Vector3 planetPos = currentPlanetVisual.transform.position;
        quizCanvas.transform.position = planetPos + Vector3.up * heightAbovePlanet;

        // Canvas luôn quay mặt về phía camera
        quizCanvas.transform.rotation = Quaternion.LookRotation(
            quizCanvas.transform.position - targetCamera.transform.position
        );
    }

    public void StartQuiz(string planetName, PlanetVisual visual)
    {
        if (answeredPlanets.Contains(planetName)) return;

        correctAnswer = planetName;
        currentPlanetVisual = visual;
        feedbackText.text = "";
        questionText.text = "Hành tinh này tên là gì?";

        List<string> options = GetRandomOptions(planetName);
        SetupButton(buttonA, options[0]);
        SetupButton(buttonB, options[1]);
        SetupButton(buttonC, options[2]);
        SetupButton(buttonD, options[3]);

        SetButtonColor(buttonA, Color.white);
        SetButtonColor(buttonB, Color.white);
        SetButtonColor(buttonC, Color.white);
        SetButtonColor(buttonD, Color.white);

        // Đặt canvas trên đầu hành tinh rồi mới hiện
        PlaceCanvasAbovePlanet();
        quizPanel.SetActive(true);
    }

    private void SetupButton(Button btn, string name)
    {
        btn.GetComponentInChildren<TextMeshProUGUI>().text = name;
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => OnAnswer(btn, name));
    }

    private void OnAnswer(Button btn, string answer)
    {
        if (answer == correctAnswer)
        {
            SetButtonColor(btn, Color.green);
            feedbackText.text = "Chính xác!";
            feedbackText.color = Color.green;

            answeredPlanets.Add(correctAnswer);

            if (currentPlanetVisual != null)
                currentPlanetVisual.ShowCorrectLabel();

            StartCoroutine(CorrectRoutine());
        }
        else
        {
            SetButtonColor(btn, Color.red);
            feedbackText.text = "Sai rồi! Thử lại nhé.";
            feedbackText.color = Color.red;
        }
    }

    private IEnumerator CorrectRoutine()
    {
        yield return new WaitForSeconds(1.5f);

        quizPanel.SetActive(false);
        feedbackText.text = "";

        if (SolarSystemFocus.Instance != null)
            SolarSystemFocus.Instance.ZoomOut();

        if (answeredPlanets.Count >= allPlanets.Length)
            StartCoroutine(ShowComplete());
    }

    private IEnumerator ShowComplete()
    {
        yield return new WaitForSeconds(2f);
        completePanel.SetActive(true);
    }

    private void SetButtonColor(Button btn, Color color)
    {
        var colors = btn.colors;
        colors.normalColor = color;
        colors.selectedColor = color;
        btn.colors = colors;
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

    public bool IsAnswered(string planetName) => answeredPlanets.Contains(planetName);
}