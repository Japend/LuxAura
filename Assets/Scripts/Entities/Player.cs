using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class Player :ClockEventReceiver{

    private List<EventEntity> planets;
    public List<EventEntity> Planets
    {
        get { return planets; }
    }

    public int Id;
    public AIType typeAI;
    public bool Deactivated { get { return deactivated; } }
    public Game currentGame;

    private AI AI;
    private bool PendingAICycle = false;
    private Effector myEffector;
    private bool deactivated;

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
            switch (typeAI)
            {
                case AIType.Dumb:
                    AI = new DumbAI();
                    break;
                case AIType.Random:
                    AI = new RandomAI();
                    break;
                case AIType.Classic:
                    AI = new ClasicAI(this);
                    break;
            }
            Clock.Instance.AddListener(this);
            myEffector = new Effector(this);
            deactivated = false;
        }
    }

    public void PreparaLista(List<EventEntity> lista)
    {
        planets = lista;
    }

    void Update()
    {
        if (Id != GlobalData.HUMAN_PLAYER)
        {
            if (PendingAICycle)
            {
                PendingAICycle = false;
                Actions act = AI.Decide();
                myEffector.Execute(act);
            }
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
        if (planets.Count <= 0)
            deactivated = true;
        if (!deactivated)
        {
            if (type == Clock.EventType.IATick && AI != null)
            {
                //print("tick ia");
                PendingAICycle = true;
                /* StopCoroutine("AICycle");
                 StartCoroutine("AICYcle");*/
            }
        }
    }

    IEnumerator AICycle()
    {
        if (typeAI != AIType.Montecarlo)
        {
            yield return null;
            Actions act = AI.Decide();
            yield return null;
            myEffector.Execute(act);
        }
        else
        {
            MontecarloAI aux = (MontecarloAI)AI;
            aux.MontecarloDecide(currentGame.GetSnapshot());
            while (!aux.Ready)
                yield return null;

#if UNITY_EDITOR
            if (aux.ActionToExecute == Actions.None)
                Debug.LogError("ERROR EN MONTECARLO: NO SE HA OBTENIDO ACCION");
#endif
            myEffector.Execute(aux.ActionToExecute);
        }
    }

    /// <summary>
    /// Returns an instance of the simplified class with the exact same state
    /// It also stores this state as a snapshot in the return instance
    /// </summary>
    /// <returns>Instsance of the simplified class in the same state and with snapshot saved</returns>
    public TPlayer GetSnapshotInstance(TEventEntity[] s_planets)
    {
        return new TPlayer(Id, typeAI, s_planets);
    }
}
