using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlanetController : MonoBehaviour
{
    public List<PlanetVisual> smallPlanets;
    public List<PlanetVisual> bigPlanets;

    public void SetPlanetZoom(PlanetVisual targetPlanet)
    {
        foreach (var planet in smallPlanets)
        {
            if(planet != targetPlanet)
                planet.ShowMarker();
        }
    }
}
