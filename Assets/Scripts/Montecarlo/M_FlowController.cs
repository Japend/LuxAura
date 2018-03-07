﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using Utilities;
using System.Threading;

[System.Serializable]
public class PlayerSettings
{
    public AIType TypeAI;
}

public class M_FlowController {

    public int TotalGamesToPlay = 1;

    private PlayerSettings test1, test2;


    public PlayerSettings[] Players;
    protected TGame currentGame;
    public TGame CurrentGame { get { return currentGame; } }

    private int gamesPlayed = 0;

    /// <summary>
    /// Creates a game and trains the given amount of times (1 by default)
    /// </summary>
    /// <param name="planetsInfo">Planets for the game</param>
    /// <param name="players">Players for the game</param>
    /// <param name="gamesToPlay">Games that will be played (default = 1)</param>
    public void StartTraining(TrainingPlanetInfo[] planetsInfo, PlayerSettings[] players, int gamesToPlay = 1)
    {
        Debug.Log("EMPEZANDO PARTIDA - CREANDO JUEGO");
        TotalGamesToPlay = gamesToPlay;

        currentGame = new TGame(players, planetsInfo);

        Thread t = new Thread(Train);
        t.Start();
    }

    /// <summary>
    /// Receives a game in its state and trains from the given state
    /// </summary>
    /// <param name="game">Game in a certain state from where the train will begin</param>
    public void StartTraining(TGame game)
    {
        currentGame = game;
        Thread t = new Thread(Train);
        t.Start();
    }

    private void Train()
    {
        bool someoneWon = false;
        int currentWinner = GlobalData.NO_PLAYER;
        int turnsToAdvance = 1;
        int turnsBetweenAITicks = (int) (GlobalData.MILISECONDS_BETWEEN__AI_TICKS / GlobalData.MILISECONDS_BETWEEN_TICKS);
        int remainingTurnsForNextAITick = turnsBetweenAITicks;

        while (gamesPlayed < TotalGamesToPlay)
        {
            Debug.Log("COMENZANDO PARTIDA " + gamesPlayed + " DE " + TotalGamesToPlay);
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
                    Debug.Log("ALGUIEN HA GANADO: " + currentWinner);
                }

                //advance the obtained amount of normal turns
                currentGame.CreateUnits(turnsToAdvance);
                //print("Se han avanzado al fin " + turnsToAdvance + " turnos");

                //if the time for an AI tick has come, do it
                remainingTurnsForNextAITick -= turnsToAdvance;
                if (remainingTurnsForNextAITick <= 0)
                {
                    Debug.Log("\n---------------------------TURNO------------------------\n");
                    Debug.Log(" Tick de IA");
                    Debug.Log("Ha ganado alguien? " + someoneWon);
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
            currentGame.Players[1].Planets.Remove(currentGame.Planets[1]);
            gamesPlayed++;
        }
    }
}
