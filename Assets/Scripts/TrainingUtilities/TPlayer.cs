using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public int Id;

    private ClasicAI AI;
    private bool PendingAICycle = false;

    /*public Player(int id, List<EventEntity> list)
    {
        planets = list;
        this.id = id;
        if (id > GlobalData.HUMAN_PLAYER)
        {
            AI = new ClasicAI(this);
            Clock.Instance.AddListener(this);
        }
    }*/

    public TPlayer(int id, List<TEventEntity> planets, bool takeSnapshot = false)
    {
        Id = id;
        this.planets = planets;
        if (takeSnapshot)
            TakeSnapshot();
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
        planets = s_planets;
    }
}
