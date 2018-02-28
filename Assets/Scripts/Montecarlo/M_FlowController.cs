using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class M_FlowController : MonoBehaviour {

    public int TotalGamesToPlay;
    public int NumPlayers;

    protected static M_FlowController instance;
    public static  M_FlowController Instance { get { return instance; } }

    protected TGame currentGame;
    public TGame CurrentGame { get { return currentGame; } }

    private int gamesPlayed = 0;

    public void StartTraining(TrainingPlanetInfo[] planetsInfo)
    {
        currentGame = new TGame(NumPlayers, planetsInfo);

        currentGame.RestoreSnapshot();
    }

    IEnumerator Train()
    {
        bool someoneWon = false;
        int turnsToAdvance = 1;
        int turnsBetweenAITicks = (int) (GlobalData.MILISECONDS_BETWEEN__AI_TICKS / GlobalData.MILISECONDS_BETWEEN_TICKS);
        int remainingTurnsForNextAITick = turnsBetweenAITicks;

        while (gamesPlayed < NumPlayers)
        {
            currentGame.RestoreSnapshot();
            while (!someoneWon)
            {
                turnsToAdvance = currentGame.CheckPendingAttacks(turnsToAdvance);
                if (turnsToAdvance > remainingTurnsForNextAITick)
                    turnsToAdvance = remainingTurnsForNextAITick;
                someoneWon = currentGame.SomeoneWon();
                currentGame.CreateUnits(turnsToAdvance);
            }
        }
    }
}
