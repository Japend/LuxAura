using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using System.Threading;

public class TPlayer
{

    #region SNAPSHOT VARIABLES
    /// <summary>
    /// This variables are used for saving and restoring the state of the object
    /// By doing that this way, we don't need to crerate new objects, thus speeding up the process
    /// ONLY THE VARIABLES THAT CAN CHANGE DURING A GAME ARE STORED
    /// </summary>
    List<TEventEntity> s_planets;
    #endregion


    private List<TEventEntity> planets;
    public List<TEventEntity> Planets
    {
        get { return planets; }
    }

    private int id;
    public int Id { get { return id; } }

    private AI AI;
    private AIType typeAI;
    private bool PendingAICycle = false;
    private bool deactivated = false;

    private bool decisionTaken = false;
    public bool DecisionTaken { get { return decisionTaken; } }
    public Actions actionToBeDone;
    private TEffector effector;

    public TPlayer(int id, List<TEventEntity> planets, AIType AI, TEventEntity[] level, bool takeSnapshot = false)
    {
        this.id = id;
        this.planets = planets;
        if (takeSnapshot)
            TakeSnapshot();
        typeAI = AI;
        initializeAI(level);
    }

    public TPlayer(int id, AIType typeAI, TEventEntity[] level)
    {
        this.id = id;
        this.typeAI = typeAI;
        initializeAI(level);
    }

    private void initializeAI(TEventEntity[] level)
    {
        switch (typeAI)
        {
            case AIType.Dumb:
                this.AI = new DumbAI();
                break;
            case AIType.Random:
            case AIType.Montecarlo: //to avoid overloading the simulation
                this.AI = new RandomAI();
                break;
            case AIType.Classic:
                this.AI = new TClasicAI(this, level);
                break;
        }
        effector = new TEffector(this, level);
    }

    public void SetPlanets(List<TEventEntity> planets, bool takeSnapshot = false)
    {
        this.planets = planets;
    }

    public bool HasLost()
    {
        return planets.Count <= 0;
    }

    public int GetCurrentUnitsNumber()
    {
        int total = 0;
        foreach (TEventEntity ent in planets)
        {
            total += ent.CurrentUnits;
        }

        return total;
    }

    public void TakeSnapshot()
    {
        s_planets = new List<TEventEntity>(planets);
    }

    public void RestoreSnapshot()
    {
        planets = new List<TEventEntity>(s_planets);
    }

    /// <summary>
    /// Will decide unless a specific action provided (in which case, the
    /// provided action will be executed)
    /// </summary>
    /// <param name="act">Optional. Action that will be taken by the player</param>
    public void Decide(Actions act = Actions.None)
    {
        if (!deactivated)
        {
            if (act != Actions.None)
                effector.Execute(act);
            else
            {
                decisionTaken = false;
                //Thread thread = new Thread(DecisionThread);
                DecisionThread();
            }
        }
    }

    //pendeinte: reutilizar thread durante la partida
    public void DecisionThread()
    {
        actionToBeDone = AI.Decide();
        effector.Execute(actionToBeDone);
        decisionTaken = true;
    }

    public void Deactivate()
    {
        deactivated = true;
    }
}
