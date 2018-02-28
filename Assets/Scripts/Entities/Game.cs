using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game {

    public static Game Instance;

    public GameObject level, attackPool;
    private Player[] players;
    public Player[] Players
    {
        get { return players; }
    }

    private EventEntity[] planets;

    public Game(EventEntity[] planets)
    {
        Instance = this;
        level = GameObject.Find("Level");
        attackPool = GameObject.Find("AttackPool");
        //LevelInfo.PrepareLevel();
        this.planets = planets;
        SetPlayers();
    }

    private void SetPlayers()
    {
       /* players = new Player[LevelInfo.GetNumberOfPlayers()];
        List<EventEntity> aux = new List<EventEntity>();
        GameObject planets = GameObject.Find("Level");

        for (int i = 0; i < LevelInfo.GetNumberOfPlayers(); i++)
        {
            aux.Clear();

            foreach (Transform child in planets.transform)
            {
                if (child.GetComponent<EventEntity>().CurrentPlayerOwner == i)
                    aux.Add(child.GetComponent<EventEntity>());
            }
            players[i] = new Player(i, new List<EventEntity>(aux));
        }*/
    }



    public void Set(Player[] arr)
    {
        players = arr;
        for (int i = 0; i < players.Length; i++)
        {
            players[i].Id = i;
        }
    }

    public int SomeoneWon()
    {
        int winner = -1;
        foreach (Transform child in level.transform)
        {
            if (winner == -1)
            {
                winner = child.GetComponent<Planet>().CurrentPlayerOwner;
            }
            else
            {
                if (winner != child.GetComponent<Planet>().CurrentPlayerOwner /*&& child.GetComponent<Planet>().CurrentPlayerOwner != GlobalData.NO_PLAYER*/)
                    return -1;
            }
        }
        if (winner == -1)
            Debug.LogError("GANADOR -1 WUUUT");
        return winner;
    }

    public TGame GetSnapshot()
    {
        TEventEntity[] s_planets = new TEventEntity[planets.Length];

        for (int i = 0; i < planets.Length; i++)
        {
            s_planets[i] = planets[i].TakeSnapshot();
        }

        TPlayer[] s_players = new TPlayer[players.Length];
        for (int i = 0; i < players.Length; i++)
        {
            s_players[i] = players[i].GetSnapshotInstance();
        }

        List<TAttackInfo> s_attacks = new List<TAttackInfo>();
        foreach (Transform att in attackPool.transform)
        {
            s_attacks.Add(att.GetComponent<Attack>().GetTrainingSnapshot());
        }
        return new TGame(s_players, s_planets, s_attacks);
    }
}
