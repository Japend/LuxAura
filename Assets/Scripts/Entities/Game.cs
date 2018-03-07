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
        TGame game = new TGame();

        for (int i = 0; i < planets.Length; i++)
        {
            s_planets[i] = planets[i].TakeSnapshot(game);
        }

        TPlayer[] s_players = new TPlayer[players.Length];
        List<TEventEntity> aux;
        for (int i = 0; i < players.Length; i++)
        {
            aux = new List<TEventEntity>();
            s_players[i] = players[i].GetSnapshotInstance(s_planets);
            foreach (TEventEntity ent in s_planets)
            {
                if (ent.CurrentPlayerOwner == s_players[i].Id)
                    aux.Add(ent);
            }
            s_players[i].SetPlanets(aux);
        }

        List<TAttackInfo> s_attacks = new List<TAttackInfo>();
        foreach (Transform att in attackPool.transform)
        {
            if(att.gameObject.activeSelf)
                s_attacks.Add(att.GetComponent<Attack>().GetTrainingSnapshot());
        }
        game.Initialize (s_players, s_planets, s_attacks);
        return game;
    }
}
