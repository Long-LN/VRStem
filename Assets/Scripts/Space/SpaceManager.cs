using System;
using UnityEngine;

[RequireComponent(typeof(SceneTrigger))]
public class SpaceManager : MonoBehaviour
{
    public static SpaceManager instance;
    

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        GetComponent<SceneTrigger>().PlayIntro();
    }

    public void EndGame()
    {
        GetComponent<SceneTrigger>().PlayOutro();
    }
}
