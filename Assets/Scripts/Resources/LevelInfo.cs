using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelInfo : MonoBehaviour {

    public Game currentGame;
    public Player[] playerArr;
    public void Start()
    {
        //LAS LISTAS SE PASAN POR REFERENCIA
        //LAS LISTAS SE PASAN POR REFERENCIA
        //LAS LISTAS SE PASAN POR REFERENCIA
        //LAS LISTAS SE PASAN POR REFERENCIA
        switch (Application.loadedLevelName)
        {
            case "Test":

                //prepare planets
                GameObject.Find("Planet").GetComponent<EventEntity>().SetParameters(GlobalData.AI_PLAYERS + 1,2);
                GameObject.Find("Planet (1)").GetComponent<EventEntity>().SetParameters(GlobalData.NO_PLAYER,2);
                GameObject.Find("Planet (2)").GetComponent<EventEntity>().SetParameters(GlobalData.NO_PLAYER,2);
                GameObject.Find("Planet (3)").GetComponent<EventEntity>().SetParameters(GlobalData.HUMAN_PLAYER, 2);
                GameObject.Find("Planet (4)").GetComponent<EventEntity>().SetParameters(GlobalData.NO_PLAYER, 2);
                GameObject.Find("Planet (5)").GetComponent<EventEntity>().SetParameters(GlobalData.NO_PLAYER, 2);
                GameObject.Find("Planet (6)").GetComponent<EventEntity>().SetParameters(GlobalData.AI_PLAYERS, 2);

                //prepare players
                List<EventEntity> list = new List<EventEntity>();
                //Player[] playerArr = new Player[3];
                //list.Add(GameObject.Find("Planet (1)").GetComponent<EventEntity>());
                list.Add(GameObject.Find("Planet").GetComponent<EventEntity>());
                playerArr[GlobalData.AI_PLAYERS + 1].PreparaLista(list);// = new Player(GlobalData.HUMAN_PLAYER, list);

                list = new List<EventEntity>();
                list.Add(GameObject.Find("Planet (6)").GetComponent<EventEntity>());
                playerArr[GlobalData.AI_PLAYERS].PreparaLista(list);// = new Player(GlobalData.AI_PLAYERS, list);

                list = new List<EventEntity>();
                list.Add(GameObject.Find("Planet (3)").GetComponent<EventEntity>());
                playerArr[GlobalData.HUMAN_PLAYER].PreparaLista(list);// = new Player(GlobalData.HUMAN_PLAYER, list);

                Game.Instance.Set(playerArr);
                break;
        }
    }

    public static int GetNumberOfPlayers()
    {
        switch (Application.loadedLevelName)
        {
            case "Test":
                return 3;
            default:
                Debug.LogError("SCNE NOT FOUND");
                return 2;
        }
    }
}
