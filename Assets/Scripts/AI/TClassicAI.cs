using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class TClasicAI : AI
{

    private TPlayer myPlayer;
    private TEventEntity[] map;
    private System.Random rand;

    public TClasicAI(TPlayer play, TEventEntity[] map)
    {
        myPlayer = play;
        this.map = map;
        rand = new System.Random();
    }

    public Actions Decide()
    {
        bool attack = false;
        bool attackNeutal = false;

        if (PlanetsNeedHealing())
        {
            return Actions.Heal;
        }

        if (PlanetsNotAtMaximmumLevel() && rand.NextDouble() > 0.5f)
        {
            return Actions.Upgrade;
        }

        if (rand.NextDouble() > 0.4f)
        {
            if (rand.NextDouble() > 0.5f)
                attack = true;
            else
                attackNeutal = true;
        }

        TEventEntity objective;
        if (ThereAreNeutralPlanets() && !attack)
        {
            //print("Hay neutrales");
            objective = TEffector.GetNearestPlanet(myPlayer, map, true);
            if (myPlayer.GetCurrentUnitsNumber() > TEffector.CountNecessaryUnitsToConquer(objective, myPlayer) * 1.5f || attackNeutal)
                return Actions.AttackNeutral;
        }
        else
        {
            objective = TEffector.GetNearestPlanet(myPlayer, map);
            if (myPlayer.GetCurrentUnitsNumber() > TEffector.CountNecessaryUnitsToConquer(objective, myPlayer) * 1.5f || attack)
            {
                return Actions.AttackEnemy;
            }
        }
        return Actions.Wait;
    }


    private bool ThereAreNeutralPlanets()
    {
        foreach (TEventEntity child in map)
        {
            if (child.CurrentPlayerOwner == GlobalData.NO_PLAYER)
                return true;
        }

        return false;
    }


    private bool PlanetsNeedHealing()
    {
        foreach (TEventEntity ent in myPlayer.Planets)
        {
            if (ent.CurrentHealth < ent.MaxHealth)
                return true;
        }
        return false;
    }

    private bool PlanetsNotAtMaximmumLevel()
    {
        foreach (TEventEntity ent in myPlayer.Planets)
        {
            if (ent.CurrentLevel < ent.MaxLevel)
                return true;
        }
        return false;
    }
}
