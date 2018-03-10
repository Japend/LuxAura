using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class MontecarloTT
{

    private M_Node root;
    public M_Node Root { get { return root; } }

    /// <summary>
    /// When this variable is set to true, Montecarlo will stop expanding and will return the best action
    /// as soon as all threads have finished
    /// </summary>
    private bool stop;

    /// <summary>
    /// Initializes the tree with the provided state at the root
    /// </summary>
    /// <param name="currentState">The state of the game from wich the simulation will start</param>
    public MontecarloTT(TGame currentState, float executionTime = GlobalData.MONTECARLO_TIMER_MILISECONDS)
    {
        root = new M_Node(currentState);
        Clock.Instance.AddTimerForMontecarlo(new System.Timers.ElapsedEventHandler(TimesUp));
        stop = false;
    }

    /// <summary>
    /// Empties the tree and makes a new one with the provided state
    /// </summary>
    /// <param name="currentState">The new state of the game from wich the simulation will start</param>
    public void ResetTree(TGame currentState)
    {
        root = new M_Node(currentState);
        stop = false; 
    }

    /// <summary>
    /// Tells the tree to stop as soon as it cans
    /// and to return the best action
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void TimesUp(object sender, System.Timers.ElapsedEventArgs e)
    {
        stop = true;
    }
}
