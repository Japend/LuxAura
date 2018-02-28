using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingPlanetInfo : MonoBehaviour {

    public int Owner, MaxLevel;
    public Vector3 position;

    void Awake()
    {
        position = transform.position;
    }

}
