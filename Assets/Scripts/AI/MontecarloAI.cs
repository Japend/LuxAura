using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MontecarloAI : AI {

    public bool Ready;
    public Utilities.Actions ActionToExecute;

    MontecarloTT simulator;
    int id;

    public MontecarloAI(int id)
    {
        this.id = id;
        simulator = new MontecarloTT(id, this);
    }

    public Utilities.Actions Decide()
    {
        Debug.LogError("Montecarlo AI SHOLDN'T use the \'Decide\' method, use the \'MontecarloDecide\' instead");
        return Utilities.Actions.None;
    }

    public void MontecarloDecide(TGame currentState)
    {
        Debug.Log("EMPEZANDO");
        Ready = false;
        ActionToExecute = Utilities.Actions.None;
        simulator.StartTreeSearch(currentState);
    }

    public void Stop()
    {
        simulator.Stop();
    }
}
