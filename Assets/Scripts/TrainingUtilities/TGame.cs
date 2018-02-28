using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class TGame : BaseGame{

    #region SNAPSHOT VARIABLES
    /// <summary>
    /// This variables are used for saving and restoring the state of the object
    /// By doing that this way, we don't need to crerate new objects, thus speeding up the process
    /// ONLY THE VARIABLES THAT CAN CHANGE DURING A GAME ARE STORED
    /// </summary>
    List<TAttackInfo> s_pendingAttacks;
    #endregion

    TPlayer[] players;
    public TPlayer[] Players {  get { return players; } }

    TEventEntity[] planets; 
    public TEventEntity[] Planets {  get { return planets; } }

    //public List<TAttackInfo> PendingAttacks { get { return pendingAttacks; } }



    public TGame(int numPlayers, TrainingPlanetInfo[] planetsInfo)
    {
        GameMutex = new Mutex();

        #region PREPARING PLANETS
        planets = new TPlanet[planetsInfo.Length];

        //instantiate planets
        for (int i = 0; i < planetsInfo.Length; i++)
        {
            planets[i] = new TPlanet(planetsInfo[i].Owner, planetsInfo[i].position, planetsInfo[i].MaxLevel, i);
        }

        //store the distances in turns between planets
        int[] aux;
        for (int i = 0; i < planets.Length; i++)
        {
            aux = new int[planets.Length];
            for (int j = 0; j < planets.Length; j++)
            {
                if (i == j)
                {
                    aux[i] = -1;
                    continue;
                }

                aux[i] = Mathf.RoundToInt(Vector3.Distance(planets[i].Position, planets[j].Position) / Attack.MOVEMENT_SPEED);
            }
            planets[i].UpdateDistances(aux);
        }
        #endregion


        #region PREPARING_PLAYERS
        players = new TPlayer[numPlayers];
        List<TEventEntity> aux2;
        for (int i = 0; i < numPlayers; i++)
        {
            aux2 = new List<TEventEntity>();
            foreach (TEventEntity planet in planets)
            {
                if (planet.CurrentPlayerOwner == i)
                    aux2.Add(planet);
            }

            players[i] = new TPlayer(i, aux2);
        }
        #endregion
        TakeSnapshot();
    }

    public TGame(TPlayer[] pl, TEventEntity[] planets, List<TAttackInfo> attacks)
    {
        GameMutex = new Mutex();
        players = pl;
        this.planets = planets;
        pendingAttacks = attacks;
        TakeSnapshot();
    }

    public bool SomeoneWon()
    {
        int currentWinner = GlobalData.NO_PLAYER;
        for (int i = 1; i < planets.Length; i++)
        {
            if (currentWinner == GlobalData.NO_PLAYER)
                currentWinner = planets[i].CurrentPlayerOwner;
            else
            {
                if (currentWinner != planets[i].CurrentPlayerOwner)
                    return false;
            }
        }

        if (currentWinner == GlobalData.NO_PLAYER)
            Debug.LogError("WINNER -1 TGAME");
        return true;
    }

    /// <summary>
    /// Advances the attacks by some turns and returns the minimum number of turns for
    /// the next attack to arrive
    /// </summary>
    /// <param name="turns">The turns the game will be advanced</param>
    /// <returns>Turns for the arrival of the next attack</returns>
    public int CheckPendingAttacks(int turns)
    {
        int result = int.MaxValue;
        for(int i = pendingAttacks.Count - 1; i > 0; i--)
        {
            pendingAttacks[i].remainingTurns -= turns;

            if (pendingAttacks[i].remainingTurns <= 0)
            {
                planets[pendingAttacks[i].Destiny].SufferAttack(pendingAttacks[i]);
                pendingAttacks.RemoveAt(i);
            }

            else if (pendingAttacks[i].remainingTurns < result)
                result = pendingAttacks[i].remainingTurns;
        }
        if (result <= 0 || result == int.MaxValue)
            result = 1;
        return result;
    }

    public void CreateUnits(int turns)
    {
        foreach(TEventEntity pl in planets)
        {
            pl.Tick(Clock.EventType.MainTick, turns);
        }
    }


    /// <summary>
    /// Stores the state of the current game to restore it later
    /// </summary>
    public void TakeSnapshot()
    {
        for (int i = 0; i < planets.Length; i++)
        {
            planets[i].TakeSnapshot();
        }

        for (int i = 0; i < planets.Length; i++)
        {
            players[i].TakeSnapshot();
        }

        s_pendingAttacks = new List<TAttackInfo>(pendingAttacks);
    }
    

    /// <summary>
    /// Restores the saved state of the game
    /// </summary>
    public void RestoreSnapshot()
    {
        for (int i = 0; i < planets.Length; i++)
        {
            planets[i].RestoreSnapshot();
        }

        for (int i = 0; i < players.Length; i++)
        {
            players[i].RestoreSnapshot();
        }

        pendingAttacks = s_pendingAttacks;
    }

    public void AddAttack(TAttackInfo att)
    {
        GameMutex.WaitOne();
        pendingAttacks.Add(att);
        GameMutex.ReleaseMutex();
    }
}
