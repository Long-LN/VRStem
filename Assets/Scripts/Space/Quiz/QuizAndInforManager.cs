using System.Collections;
using NUnit.Framework;
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
        bigPlanetVisual = planetController.bigPlanets.Find(planet => planet.name == planetName);
        bool isAnswered = PlanetQuiz.Instance.IsAnswered(planetName);
        if(!isAnswered)
            StartCoroutine(ShowDelayQuiz());
        else
        {
            bigPlanetVisual.ShowInfo();
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
