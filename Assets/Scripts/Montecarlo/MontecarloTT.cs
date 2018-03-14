using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using System.Threading;

public class MontecarloTT
{

    /// <summary>
    /// When this variable is set to true, Montecarlo will stop expanding and will return the best action
    /// as soon as all threads have finished
    /// </summary>
    private bool stop;

    /// <summary>
    /// For how long the algorithm will search for the best posible action
    /// </summary>
    private float timeToPlay;

    /// <summary>
    /// The tree used by the algorithm
    /// </summary>
    private MontecarloTree tree;

    /// <summary>
    /// The id of the player that will execute this method
    /// </summary>
    private int id;

    /// <summary>
    /// Maximum number of concurrnt simulations
    /// </summary>
    private int maxSimulations;

    /// <summary>
    /// Simulations that are running in the background
    /// </summary>
    private int currentActiveSimulations;

    /// <summary>
    /// Will be used to secure the currentActiveSimulations atribute
    /// </summary>
    Mutex mMutex;

    /// <summary>
    /// The best action found by montecarlo
    /// Will remian null unil the simulation stops
    /// </summary>
    public Actions bestAction;

    /// <summary>
    /// Class where the simulation results will be deployed
    /// </summary>
    MontecarloAI support;


    int maxNodo;

    /// <summary>
    /// Initilices the class creating a tree but does NOT start the simulation
    /// </summary>
    /// <param name="playerId">Id of the player that will execute the tree</param>
    /// <param name="support">Montecarlo AI instance that will receive the results</param>
    /// <param name="executionTime">Miliseconds the simulation will take. Default = 1500</param>
    public MontecarloTT(int playerId, MontecarloAI support, float executionTime = GlobalData.MONTECARLO_TIMER_MILISECONDS)
    {
        timeToPlay = executionTime;
        id = playerId;
        int dummy;
        ThreadPool.GetMaxThreads(out maxSimulations, out dummy);
        mMutex = new Mutex();
        this.support = support;
    }

    /// <summary>
    /// Empties the tree and makes a new one with the provided state
    /// </summary>
    /// <param name="currentState">The new state of the game from wich the simulation will start</param>
    public void StartTreeSearch(TGame currentState)
    {
        stop = false;
        Clock.Instance.AddTimerForMontecarlo(new System.Timers.ElapsedEventHandler(TimesUp));
        //We substract one because the mother thread will always be there
        maxSimulations--;
        currentActiveSimulations = 0;
        ThreadPool.QueueUserWorkItem(MotherThread, currentState);
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
        support.ActionToExecute = tree.GetBestAction();
        Debug.Log("Se ha llegado hasta el nodo " + maxNodo);
        Debug.Log("ACTION TO EXECUTE = " + support.ActionToExecute);
        support.Ready = true;
    }


    /// <summary>
    /// The thread that will control the montecarlo execution
    /// </summary>s
    /// <param name="info">TGame with the current state of the game (the simullation will continue from there)</param>
    private void MotherThread(System.Object info)
    {
        maxNodo = 0;
        tree = new MontecarloTree((TGame)info, id);
        M_Node aux;
        while (!stop)
        {
            if (currentActiveSimulations <= maxSimulations)
            {
                aux = tree.SearchForNextNode();
                if (aux.Position > maxNodo)
                    maxNodo = aux.Position;
                mMutex.WaitOne();
                currentActiveSimulations++;
                mMutex.ReleaseMutex();
                ThreadPool.QueueUserWorkItem(Simulate, aux);
            }
            /*else
                Thread.Sleep(40);*/
        }
    }


    private void Simulate(System.Object info)
    {
        M_FlowController flow = new M_FlowController();
        M_Node node = (M_Node)info;
        //Debug.Log("Empezando entrenamiento de nodo " + node.Position);
        flow.StartTrainingInThisThread(node.State);
        #if UNITY_EDITOR
        if (node.State.SomeoneWon() == GlobalData.NO_PLAYER)
            Debug.LogError("NADIE GANO AL EJECUTAR LA SIMULACION");
        #endif

        if (node.State.SomeoneWon() == id)
        {
            //Debug.Log("Terminando entrenamiento de nodo " + node.Position);
            node.Score += GlobalData.MONTECARLO_REWARD;
        }
        else
            node.Score += GlobalData.MONTECARLO_PENALIZATION;
        //Debug.Log("Terminando entrenamiento de nodo " + node.Position + " con "  + node.Score + " puntos" );
        tree.BackpropagateScore(node);
        node.State.RestoreSnapshot();
        mMutex.WaitOne();
        currentActiveSimulations--;
        mMutex.ReleaseMutex();
    }

    public void Stop()
    {
        stop = true;
    }
}
