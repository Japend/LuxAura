using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class OLD_TClasicAI : AI
{

    private OLD_TPlayer myPlayer;
    private OLD_TEventEntity[] map;

    public OLD_TClasicAI(OLD_TPlayer play, OLD_TEventEntity[] map)
    {
        myPlayer = play;
        this.map = map;
    }

    public Actions Decide()
    {
        bool attack = false;
        bool attackNeutal = false;

        if (PlanetsNeedHealing())
        {
            return Actions.Heal;
        }

        if (PlanetsNotAtMaximmumLevel() && Random.value > 0.5f)
        {
            return Actions.Upgrade;
        }

        if (Random.value > 0.4f)
        {
            if (Random.value > 0.5f)
                attack = true;
            else
                attackNeutal = true;
        }

        OLD_TEventEntity objective;
        if (ThereAreNeutralPlanets() && !attack)
        {
            //print("Hay neutrales");
            objective = OLD_TEffector.GetNearestPlanet(myPlayer, map, true);
            if (myPlayer.GetCurrentUnitsNumber() > OLD_TEffector.CountNecessaryUnitsToConquer(objective, myPlayer) * 1.5f || attackNeutal)
                return Actions.AttackNeutral;
        }
        else
        {
            objective = OLD_TEffector.GetNearestPlanet(myPlayer, map);
            if (myPlayer.GetCurrentUnitsNumber() > OLD_TEffector.CountNecessaryUnitsToConquer(objective, myPlayer) * 1.5f || attack)
            {
                return Actions.AttackEnemy;
            }
        }
        return Actions.Wait;
    }


    private bool ThereAreNeutralPlanets()
    {
        foreach (OLD_TEventEntity child in map)
        {
            if (child.CurrentPlayerOwner == GlobalData.NO_PLAYER)
                return true;
        }

        return false;
    }


    private bool PlanetsNeedHealing()
    {
        foreach (OLD_TEventEntity ent in myPlayer.Planets)
        {
            if (ent.CurrentHealth < ent.MaxHealth)
                return true;
        }
        return false;
    }

    private bool PlanetsNotAtMaximmumLevel()
    {
        foreach (OLD_TEventEntity ent in myPlayer.Planets)
        {
            if (ent.CurrentLevel < ent.MaxLevel)
                return true;
        }
        return false;
    }
}
