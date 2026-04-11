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
        smallPlanetVisual = planetController.smallPlanets.Find(planet => planet.name == planetName);
        bigPlanetVisual   = planetController.bigPlanets.Find(planet => planet.name == planetName);

        bool isAnswered = PlanetQuiz.Instance.IsAnswered(planetName);
        if (!isAnswered)
            StartCoroutine(ShowDelayQuiz());
        else
        {
            // Vector3 planetPos = SolarSystemFocus.Instance.GetFocusWorldPos();
            Camera cam = PlanetQuiz.Instance.targetCamera;
            bigPlanetVisual.ShowInfo(cam);
        }
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
}