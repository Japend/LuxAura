using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player :ClockEventReceiver{

    private List<EventEntity> planets;
    public List<EventEntity> Planets
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

    void Start()
    {
        if (Id > GlobalData.HUMAN_PLAYER)
        {
            AI = new ClasicAI(this);
            Clock.Instance.AddListener(this);
        }
    }

    public void PreparaLista(List<EventEntity> lista)
    {
        planets = lista;
    }

    void Update()
    {
        if (PendingAICycle)
        {
            PendingAICycle = false;
            AI.Decide();
        }

    }
    public bool HasLost()
    {
        return planets.Count <= 0;
    }

    public int SelectAllUnits()
    {
        int total = 0;
        foreach (EventEntity ent in planets)
        {
            total += ent.SelectUnits(true);
        }

        return total;
    }

    public int GetCurrentUnitsNumber()
    {
        int total = 0;
        foreach (EventEntity ent in planets)
        {
            total += ent.CurrentUnits + ent.SelectedUnits;
        }

        return total;
    }

    public override void Tick(Clock.EventType type)
    {
        
        if (type == Clock.EventType.IATick && AI != null)
        {
            //print("tick ia");
            PendingAICycle = true;
           /* StopCoroutine("AICycle");
            StartCoroutine("AICYcle");*/
        }
    }

    IEnumerator AICycle()
    {
        yield return null;
        AI.Decide();
    }

    /// <summary>
    /// Returns an instance of the simplified class with the exact same state
    /// It also stores this state as a snapshot in the return instance
    /// </summary>
    /// <returns>Instsance of the simplified class in the same state and with snapshot saved</returns>
    public TPlayer GetSnapshotInstance()
    {
        return null;
    }
}
