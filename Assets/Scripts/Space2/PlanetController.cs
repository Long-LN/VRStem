using UnityEngine;

public class PlanetController : MonoBehaviour
{
    public PlanetVisual[] planets;

    public void SetPlanetZoom(PlanetVisual targetPlanet)
    {
        foreach (var planet in planets)
        {
            if(planet != targetPlanet)
                planet.ShowMarker();
        }
    }
}
