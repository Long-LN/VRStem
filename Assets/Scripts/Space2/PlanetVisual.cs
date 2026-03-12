using UnityEngine;

public class PlanetVisual : MonoBehaviour
{
    public GameObject marker;
    public GameObject model;

    public void ShowMarker()
    {
        marker.SetActive(true);
        model.SetActive(false);
    }

    public void ShowModel()
    {
        marker.SetActive(false);
        model.SetActive(true);
        model.transform.position = marker.transform.position;
    }
}