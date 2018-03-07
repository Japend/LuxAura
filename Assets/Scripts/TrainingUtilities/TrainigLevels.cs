using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;


public class TrainigLevels : MonoBehaviour
{
    public string levelToLoad;
    OLD_M_FlowController flow;
    void Start()
    {
        GameObject level = GameObject.Find(levelToLoad);

        if (level == null)
        {
            Debug.LogError("Parent level not found");
            return;
        }

        TrainingPlanetInfo[] planets = new TrainingPlanetInfo[level.transform.childCount];

        for (int i = 0; i < level.transform.childCount; i++)
        {
            planets[i] = level.transform.GetChild(i).GetComponent<TrainingPlanetInfo>();
        }

        flow = new OLD_M_FlowController();
        flow.StartTraining(planets);
    }
}