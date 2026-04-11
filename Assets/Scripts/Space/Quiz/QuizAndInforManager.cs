using System.Collections;
using UnityEngine;

public class QuizAndInforManager : MonoBehaviour
{
    SolarSystemFocus solarSystemFocus;
    public PlanetController planetController;
    public float delayTime;
    public PlanetVisual bigPlanetVisual;
    public PlanetVisual smallPlanetVisual;

    void Start()
    {
        solarSystemFocus = SolarSystemFocus.Instance;
    }

    public void ShowPanel(string planetName)
    {
        Debug.Log(planetName);
        smallPlanetVisual = planetController.smallPlanets.Find(planet => planet.name == planetName);
        bigPlanetVisual   = planetController.bigPlanets.Find(planet => planet.name == planetName);

        bool isAnswered = PlanetQuiz.Instance.IsAnswered(planetName);
        Debug.Log($"[QuizAndInforManager] {planetName} isAnswered={isAnswered}");

        if (!isAnswered)
            StartCoroutine(ShowDelayQuiz());
        else
            StartCoroutine(ShowDelayInfo(planetName)); // [SỬA] Gọi coroutine mới thay vì ShowInfo trực tiếp
    }

    public void HidePanel(string planetName)
    {
        bigPlanetVisual = planetController.bigPlanets.Find(planet => planet.name == planetName);
        bigPlanetVisual.HideInfo();
    }

    IEnumerator ShowDelayQuiz()
    {
        yield return new WaitForSeconds(0.1f);
        Debug.Log("ShowQuiz");
        PlanetQuiz.Instance.StartQuiz(smallPlanetVisual.planetName, smallPlanetVisual);
    }

    // [THÊM MỚI] TH2: hành tinh đã trả lời → hiện info + phát sequence mô tả, không zoom out
    IEnumerator ShowDelayInfo(string planetName)
    {
        yield return new WaitForSeconds(0.1f);

        Debug.Log($"[QuizAndInforManager] TH2 - Hiện info + phát thuyết minh cho: {planetName}");

        // Hiện bảng thông tin
        bigPlanetVisual.ShowInfo();

        // Phát sequence mô tả hành tinh, không có callback zoom out
        if (AnswerTrigger.Instance != null)
        {
            Debug.Log($"[QuizAndInforManager] Gọi AnswerTrigger.PlayDescription cho: {planetName}");
            AnswerTrigger.Instance.PlayDescription(planetName);
        }
        else
        {
            Debug.LogWarning("[QuizAndInforManager] AnswerTrigger.Instance là null!");
        }
    }
}