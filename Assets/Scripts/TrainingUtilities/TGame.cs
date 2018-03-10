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

    private bool weHaveAWinner;
    public bool WeHaveAWinner { get { return weHaveAWinner; } }
    private int winner;
    public int Winner { get { return winner; } }


    //public List<TAttackInfo> PendingAttacks { get { return pendingAttacks; } }

    public TGame(PlayerSettings[] players, TrainingPlanetInfo[] planetsInfo)
    {
        GameMutex = new Mutex();
        Debug.Log("EMPEZANDO PARTIDA - CREANDO PLANETAS");
        #region PREPARING PLANETS
        planets = new TPlanet[planetsInfo.Length];

        //instantiate planets
        for (int i = 0; i < planetsInfo.Length; i++)
        {
            planets[i] = new TPlanet(planetsInfo[i].Owner, planetsInfo[i].position, planetsInfo[i].MaxLevel, i, this);
            Debug.Log("Creando planeta " + i + " que pertenece al jugador " + planetsInfo[i].Owner + " situado en " + planetsInfo[i].position);
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
                    Debug.Log("El planeta " + i + " esta a una distancia de " + Vector3.Distance(planets[i].Position, planets[j].Position) + " al planeta " + j + " o, en turnos: " + aux[i]); 
                    continue;
                }

                aux[i] = Utilities.Utilities.GetDistanceInTurns(planets[i].Position, planets[j].Position);
                Debug.Log("El planeta " + i + " esta a una distancia de " + Vector3.Distance(planets[i].Position, planets[j].Position) + " al planeta " + j + " o, en turnos: " + aux[i]); 
            }
            planets[i].UpdateDistances(aux);
        }
        #endregion

        Debug.Log("EMPEZANDO PARTIDA - CREANDO JUGADORES");
        #region PREPARING_PLAYERS
        TPlayer[] mplayers = new TPlayer[players.Length];
        List<TEventEntity> aux2;
        for (int i = 0; i < players.Length; i++)
        {
            aux2 = new List<TEventEntity>();
            foreach (TEventEntity planet in planets)
            {
                if (planet.CurrentPlayerOwner == i)
                    aux2.Add(planet);
            }

            mplayers[i] = new TPlayer(i, aux2, players[i].TypeAI, planets, true);
            Debug.Log("Jugador " + i + " creado con " + aux2.Count + " planetas e IA " + players[i].TypeAI);
        }
        this.players = mplayers;
        #endregion
        pendingAttacks = new List<TAttackInfo>();
        TakeSnapshot();
        winner = GlobalData.NO_PLAYER;
        weHaveAWinner = false;
    }

    public TGame(TPlayer[] pl, TEventEntity[] planets, List<TAttackInfo> attacks)
    {
        GameMutex = new Mutex();
        players = pl;
        this.planets = planets;
        pendingAttacks = attacks;
        TakeSnapshot();
        winner = GlobalData.NO_PLAYER;
        weHaveAWinner = false;
    }

    public TGame()
    {
        GameMutex = new Mutex();
    }


    public void Initialize(TPlayer[] pl, TEventEntity[] planets, List<TAttackInfo> attacks)
    {
        players = pl;
        this.planets = planets;
        pendingAttacks = attacks;
        winner = GlobalData.NO_PLAYER;
        weHaveAWinner = false;
        TakeSnapshot();
    }
    /// <summary>
    /// Checks if there is a winner and deactivates the players that have lost
    /// </summary>
    /// <returns>NO_PLAYER if there is no winner, winner id if there is one</returns>
    public int SomeoneWon()
    {
        //first, we check each player to deactivate it if has lost already
        foreach (TPlayer play in players)
        {
            if (play.HasLost())
            {
                play.Deactivate();
            }
        }

        int currentWinner = GlobalData.NO_PLAYER;
        for (int i = 0; i < planets.Length; i++)
        {
            if (currentWinner == GlobalData.NO_PLAYER)
                currentWinner = planets[i].CurrentPlayerOwner;
            else
            {
                if (currentWinner != planets[i].CurrentPlayerOwner && planets[i].CurrentPlayerOwner != GlobalData.NO_PLAYER)
                    return GlobalData.NO_PLAYER;
            }
        }
        if (currentWinner != GlobalData.NO_PLAYER)
        {
            weHaveAWinner = true;
            winner = currentWinner;
        }
        return currentWinner;
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
        for(int i = pendingAttacks.Count - 1; i >= 0; i--)
        {
            pendingAttacks[i].remainingTurns -= turns;
            Debug.Log("Al ataque de " + pendingAttacks[i].Player + " hacia " + pendingAttacks[i].Destiny + " con " + pendingAttacks[i].Units + " le faltan " + pendingAttacks[i].remainingTurns);
            if (pendingAttacks[i].remainingTurns <= 0)
            {
                Debug.Log("El planeta " + pendingAttacks[i].Destiny + " tiene unidades/salud/exp " + planets[pendingAttacks[i].Destiny].CurrentUnits + " / " + planets[pendingAttacks[i].Destiny].CurrentHealth + " / " + planets[pendingAttacks[i].Destiny].CurrentExp);
                Debug.Log("Y sufre un ataque de " + pendingAttacks[i].Units);
                planets[pendingAttacks[i].Destiny].SufferAttack(pendingAttacks[i]);
                Debug.Log("Ahora el planeta tiene unidades/salud/exp " + planets[pendingAttacks[i].Destiny].CurrentUnits + " / " + planets[pendingAttacks[i].Destiny].CurrentHealth + " / " + planets[pendingAttacks[i].Destiny].CurrentExp);
                pendingAttacks.RemoveAt(i);
            }

            else if (pendingAttacks[i].remainingTurns < result)
                result = pendingAttacks[i].remainingTurns;
        }
        if (result <= 0 || result == int.MaxValue)
            result = 1;
        return result;
    }

    /// <summary>
    /// Advances the amount of turns provided as normal turns
    /// </summary>
    /// <param name="turns">The turns that will be played</param>
    public void CreateUnits(int turns)
    {
        foreach(TEventEntity pl in planets)
        {
            pl.Tick(turns);
        }
    }


    /// <summary>
    /// Triggers an AI tick
    /// </summary>
    public void AITick()
    {
        foreach (TPlayer play in players)
        {
            if (!play.HasLost())
            {
                play.Decide();
                Debug.Log("Jugador " + play.Id + " decide " + play.actionToBeDone);
            }
        }
    }

    /// <summary>
    /// Simulates an AI tick but the provided player will take the given decision
    /// </summary>
    /// <param name="idPlayerFixedDecision">The player that will execute the given action</param>
    /// <param name="act">The action to be executed by the provided player</param>
    public void AITick(int idPlayerFixedDecision, Utilities.Actions act)
    {

        for (int i = 0; i < players.Length; i++)
        {
            if (!players[i].HasLost())
            {
                if (i == idPlayerFixedDecision)
                {
                    players[i].Decide(act);
                }
                else
                    players[i].Decide();

            }
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

        for (int i = 0; i < players.Length; i++)
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

        pendingAttacks = new List<TAttackInfo>(s_pendingAttacks);
    }

    public void AddAttack(TAttackInfo att)
    {
        Debug.Log("Jugador " + att.Player + " añade ataque para planeta " + att.Destiny + " con " + att.Units + " que tardara " + att.remainingTurns);
        GameMutex.WaitOne();
        pendingAttacks.Add(att);
        GameMutex.ReleaseMutex();
    }

    public bool EveryoneDecided()
    {
        foreach (TPlayer play in players)
        {
            if (!play.DecisionTaken)
                return false;
        }
        return true;
    }
}
