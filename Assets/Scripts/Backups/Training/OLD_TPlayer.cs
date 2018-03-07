using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using System.Threading;

public class OLD_TPlayer
{

    #region SNAPSHOT VARIABLES
    /// <summary>
    /// This variables are used for saving and restoring the state of the object
    /// By doing that this way, we don't need to crerate new objects, thus speeding up the process
    /// ONLY THE VARIABLES THAT CAN CHANGE DURING A GAME ARE STORED
    /// </summary>
    List<OLD_TEventEntity> s_planets;
    #endregion


    private List<OLD_TEventEntity> planets;
    public List<OLD_TEventEntity> Planets
    {
        get { return planets; }
    }

    private int id;
    public int Id { get { return id; } }

    private AI AI;
    private bool PendingAICycle = false;
    private bool deactivated = false;

    private bool decisionTaken = false;
    public bool DecisionTaken { get { return decisionTaken; } }
    public Actions actionToBeDone;
    private OLD_TEffector effector;

    public OLD_TPlayer(int id, List<OLD_TEventEntity> planets, AIType AI, OLD_TEventEntity[] level, bool takeSnapshot = false)
    {
        this.id = id;
        this.planets = planets;
        if (takeSnapshot)
            TakeSnapshot();

        switch (AI)
        {
            case AIType.Dumb:
                this.AI = new DumbAI();
                break;
            case AIType.Random:
                this.AI = new RandomAI();
                break;
            case AIType.Classic:
                this.AI = new OLD_TClasicAI(this, level);
                break;
        }
        effector = new OLD_TEffector(this, level);
    }

    public bool HasLost()
    {
        return planets.Count <= 0;
    }

    public int GetCurrentUnitsNumber()
    {
        int total = 0;
        foreach (OLD_TEventEntity ent in planets)
        {
            total += ent.CurrentUnits;
        }

        return total;
    }

    public void TakeSnapshot()
    {
        s_planets = new List<OLD_TEventEntity>(planets);
    }

    public void RestoreSnapshot()
    {
        planets = new List<OLD_TEventEntity>(s_planets);
    }

    public void Decide()
    {
        if (!deactivated)
        {
            decisionTaken = false;
            //Thread thread = new Thread(DecisionThread);
            DecisionThread();
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
