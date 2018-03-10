using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;


public class OLD_M_FlowController : MonoBehaviour
{

    public int TotalGamesToPlay;
    public PlayerSettings[] Players;

    protected static OLD_M_FlowController instance;
    public static OLD_M_FlowController Instance { get { return instance; } }

    protected OLD_TGame currentGame;
    public OLD_TGame CurrentGame { get { return currentGame; } }

    private int gamesPlayed = 0;

    public void Awake()
    {
        instance = this;
    }

    public void StartTraining(TrainingPlanetInfo[] planetsInfo)
    {
        print("EMPEZANDO PARTIDA - CREANDO JUEGO");
        currentGame = new OLD_TGame(Players, planetsInfo);


        StartCoroutine("Train");
    }

    IEnumerator Train()
    {
        bool someoneWon = false;
        int currentWinner = GlobalData.NO_PLAYER;
        int turnsToAdvance = 1;
        int turnsBetweenAITicks = (int)(GlobalData.MILISECONDS_BETWEEN__AI_TICKS / GlobalData.MILISECONDS_BETWEEN_TICKS);
        int remainingTurnsForNextAITick = turnsBetweenAITicks;

        while (gamesPlayed < TotalGamesToPlay)
        {
            print("COMENZANDO PARTIDA " + gamesPlayed + " DE " + TotalGamesToPlay);
            currentGame.RestoreSnapshot();
            someoneWon = false;
            while (!someoneWon)
            {

                //check pending attacks and get turns for the arrival of the next attack
                turnsToAdvance = currentGame.CheckPendingAttacks(turnsToAdvance);

                //if the AI tick would happen before the attack arrived, we advance only
                //the necessary turns to trigger the AI tick
                if (turnsToAdvance > remainingTurnsForNextAITick)
                    turnsToAdvance = remainingTurnsForNextAITick;

                //print("Avazamos " + turnsToAdvance + " turnos faltando " + remainingTurnsForNextAITick + " turnos para el AITick");

                //check for victory (IMPORTANT TO DO IT AFTER THE ATTACKS AND NOT BEFORE)
                currentWinner = currentGame.SomeoneWon();
                if (currentWinner != GlobalData.NO_PLAYER)
                {
                    someoneWon = true;
                    print("ALGUIEN HA GANADO: " + currentWinner);
                }

                //advance the obtained amount of normal turns
                currentGame.CreateUnits(turnsToAdvance);
                //print("Se han avanzado al fin " + turnsToAdvance + " turnos");

                //if the time for an AI tick has come, do it
                remainingTurnsForNextAITick -= turnsToAdvance;
                if (remainingTurnsForNextAITick <= 0)
                {
                    print("\n---------------------------TURNO------------------------\n");
                    print(" Tick de IA");
                    print("Ha ganado alguien? " + someoneWon);
                    currentGame.AITick();
                    remainingTurnsForNextAITick = turnsBetweenAITicks;
                }

                /* while (!currentGame.EveryoneDecided())
                 {
                     print("Esperando decision");
                     yield return null;
                 }*/
                //yield return null;
            }
            gamesPlayed++;
            yield return null;
        }
    }
}
