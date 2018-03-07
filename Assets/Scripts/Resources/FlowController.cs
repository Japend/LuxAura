﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowController : MonoBehaviour {

    private static FlowController instance;
    public static FlowController Instance
    {
        get { return FlowController.Instance; }
    }

    public GameObject VictoryText;
    private Game currentGame;

    private float remainingTime;
    private M_FlowController test;

    void Awake()
    {
        instance = this;
        GameObject aux = GameObject.Find("Level");
        EventEntity[] planets = new EventEntity[aux.transform.childCount];
        for (int i = 0; i < aux.transform.childCount; i++)
        {
            planets[i] = aux.transform.GetChild(i).GetComponent<EventEntity>();
            planets[i].Id = i;
        }

        currentGame = new Game(planets);

        remainingTime = 5f;
        test = new M_FlowController();
    }
	
	// Update is called once per frame
	void Update () {
        if (currentGame.SomeoneWon() != -1)
        {
            Clock.Instance.OnApplicationQuit();
            VictoryText.SetActive(true);
        }

        remainingTime -= Time.deltaTime;
        if (remainingTime <= 0)
        {
            remainingTime = 5;
            test.StartTraining(currentGame.GetSnapshot());
        }
	}


}
