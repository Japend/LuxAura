using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class ClasicAI : AI
{

    private Player myPlayer;
    private GameObject map;

    public ClasicAI(Player play)
    {
        myPlayer = play;
        map = GameObject.Find("Level");
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

        EventEntity objective;
        if (ThereAreNeutralPlanets() && !attack)
        {
            objective = Effector.GetNearestPlanet(map, myPlayer, true);
            if (myPlayer.GetCurrentUnitsNumber() > Effector.CountNecessaryUnitsToConquer(objective, myPlayer) * 1.5f || attackNeutal)
            {
                return Actions.AttackNeutral;
            }
        }
        else
        {
            objective = Effector.GetNearestPlanet(map, myPlayer);
            if (myPlayer.GetCurrentUnitsNumber() > Effector.CountNecessaryUnitsToConquer(objective, myPlayer) * 1.5f || attack)
            {
                return Actions.AttackEnemy;
            }
        }
        return Actions.Wait;
    }


    private bool ThereAreNeutralPlanets()
    {
        foreach (Transform child in map.transform)
        {
            if (child.GetComponent<EventEntity>().CurrentPlayerOwner == GlobalData.NO_PLAYER)
                return true;
        }

        return false;
    }

    private bool PlanetsNeedHealing()
    {
        foreach (EventEntity ent in myPlayer.Planets)
        {
            if (ent.CurrentHealth < ent.MaxHealth)
                return true;
        }
        return false;
    }

    private bool PlanetsNotAtMaximmumLevel()
    {
        foreach (EventEntity ent in myPlayer.Planets)
        {
            if (ent.CurrentLevel < ent.MaxLevel)
                return true;
        }
        return false;
    }
}
