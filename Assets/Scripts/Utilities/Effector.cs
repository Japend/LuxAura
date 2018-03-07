using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

/// <summary>
/// Receives actions to do and executes them
/// </summary>
public class Effector{

    Player myPlayer;
    GameObject map;

    public Effector(Player play)
    {
       myPlayer = play;
       map = GameObject.Find("Level");
    }

    public void Execute(Actions action)
    {
        switch (action)
        {
            case Actions.Wait:
            default:
                return;
            case Actions.Heal:
                HealMyPlanets();
                return;
            case Actions.Upgrade:
                LevelUpMyPlanets();
                return;
            case Actions.AttackNeutral:
                //print("Hay neutrales");
                EventEntity objective = GetNearestPlanet(map, myPlayer, true);
                Attack(objective, Random.value > 0.25f);
                return;
            case Actions.AttackEnemy:
                objective = Effector.GetNearestPlanet(map, myPlayer);
                Attack(objective, Random.value > 0.25f);
                return;
        }


    }

    private void HealMyPlanets()
    {

        foreach (Planet ent in myPlayer.Planets)
        {
            if (ent.CurrentHealth < ent.MaxHealth)
            {
                ent.UseUnits(new AttackInfo(ent.gameObject, ent.gameObject, myPlayer.Id, ent.MaxHealth - ent.CurrentHealth), true);
            }
        }
    }


    private void LevelUpMyPlanets()
    {
        //will choose the planet closer to be leveled up or the first one it has in the list
        EventEntity objective = null;
        int currentExpNeeded = int.MaxValue;
        foreach (EventEntity ent in myPlayer.Planets)
        {
            if (ent.ExpForNextLevel - ent.CurrentExp < currentExpNeeded)
            {
                objective = ent;
                currentExpNeeded = ent.ExpForNextLevel - ent.CurrentExp;
            }
        }

        Attack(objective, true);
    }


    /// <summary>
    /// Will send 1.5 times the units necessary to conquer or turn neutral a planet
    /// The units will be sent from the nearest planets
    /// </summary>
    /// <param name="objective">Entity that will be attacked</param>
    /// <param name="randomNumber">If true, a random number of units will be sent</param>
    /// <param name="turnNeutral">If true, the planet wont be conquered, but turn neutral</param>
    private void Attack(EventEntity objective, bool randomNumber = false, bool turnNeutral = false)
    {
        int objectiveUnits = CountNecessaryUnitsToConquer(objective, randomNumber, turnNeutral);
        List<EventEntity> planets = getPlanetsSortedByDistance(objective);

        for (int i = 0; i < planets.Count; i++)
        {
            objectiveUnits -= planets[i].CurrentUnits;
            planets[i].UseUnits(new AttackInfo(planets[i].gameObject, objective.gameObject, myPlayer.Id, planets[i].CurrentUnits), true);
            if (objectiveUnits <= 0)
                return;
        }

    }



    /// <summary>
    /// Returns necessary units to conquer or turn neutral
    /// </summary>
    /// <param name="ent">Entity we want to know abouta</param>
    /// <param name="turnNeutral">If true will return units to turn neutral</param>
    /// <returns></returns>
    private int CountNecessaryUnitsToConquer(EventEntity ent, bool randomNumber = false, bool turnNeutral = false)
    {
        int objective = 0;

        if (ent.CurrentPlayerOwner == myPlayer.Id)
        {
            if (ent.CurrentLevel >= ent.MaxLevel)
                return 0;
            else
            {
                objective = ent.ExpForNextLevel - ent.CurrentExp;
            }
        }
        if ((ent.CurrentPlayerOwner == GlobalData.NO_PLAYER && ent.CurrentContestantId == myPlayer.Id) || (ent.CurrentPlayerOwner == GlobalData.NO_PLAYER && ent.CurrentContestantId == GlobalData.NO_PLAYER))
        {
            objective = ent.ExpForNextLevel - ent.CurrentExp;
        }
        else if (ent.CurrentPlayerOwner == GlobalData.NO_PLAYER && ent.CurrentContestantId != myPlayer.Id)
        {
            objective = ent.ExpForNextLevel + ent.CurrentHealth;
        }
        else
        {
            if (turnNeutral)
                objective = ent.CurrentUnits + ent.CurrentHealth;
            else
                objective = ent.CurrentHealth + ent.CurrentUnits + ent.GetExpForLEvel(0);
        }

        if (randomNumber)
            return Mathf.RoundToInt(objective * (1.4f - Random.value));
        else
            return objective;
    }

    private List<EventEntity> getPlanetsSortedByDistance(EventEntity refernce)
    {
        List<EventEntity> result = new List<EventEntity>();
        EventEntity aux, aux2;
        float currentDistance = float.PositiveInfinity;
        result.Add(myPlayer.Planets[0]);

        for (int i = 1; i < myPlayer.Planets.Count; i++)
        {
            aux = myPlayer.Planets[i];
            aux2 = aux;
            for (int j = 0; i < result.Count; j++)
            {
                if (Vector3.Distance(aux.transform.position, refernce.transform.position) < Vector3.Distance(result[j].transform.position, refernce.transform.position))
                {
                    aux2 = result[j];
                    result[j] = aux;
                    aux = aux2;
                }
            }
            result.Add(aux);
        }

        return result;
    }


    /// <summary>
    /// Returns nearest planet to aany planet that belongs to the player
    /// </summary>
    /// <param name="returnNeutral"></param>
    /// <returns></returns>
    public static EventEntity GetNearestPlanet(GameObject map, Player myPlayer, bool returnNeutral = false)
    {
        GameObject aux = null;
        float currentDistance = float.PositiveInfinity;
        foreach (Transform child in map.transform)
        {
            if (!myPlayer.Planets.Contains(child.gameObject.GetComponent<EventEntity>()))
            {
                for (int i = 0; i < myPlayer.Planets.Count; i++)
                {
                    if (Vector3.Distance(child.position, myPlayer.Planets[i].transform.position) < currentDistance)
                    {
                        if (returnNeutral)
                        {
                            if (child.GetComponent<EventEntity>().CurrentPlayerOwner == GlobalData.NO_PLAYER)
                            {
                                aux = child.gameObject;
                                currentDistance = Vector3.Distance(child.position, myPlayer.Planets[i].transform.position);
                            }
                        }
                        else
                        {
                            if (child.GetComponent<EventEntity>().CurrentPlayerOwner != GlobalData.NO_PLAYER)
                            {
                                aux = child.gameObject;
                                currentDistance = Vector3.Distance(child.position, myPlayer.Planets[i].transform.position);
                            }
                        }
                    }
                }
            }
        }

        if (aux == null)
        {
            Debug.LogError("Error al localizar planeta cercano");
            return myPlayer.Planets[0];
        }
        else
            return aux.GetComponent<EventEntity>();
    }


    /// <summary>
    /// Returns necessary units to conquer or turn neutral
    /// </summary>
    /// <param name="ent">Entity we want to know abouta</param>
    /// <param name="turnNeutral">If true will return units to turn neutral</param>
    /// <returns></returns>
    public static int CountNecessaryUnitsToConquer(EventEntity ent, Player myPlayer, bool randomNumber = false, bool turnNeutral = false)
    {
        int objective = 0;

        if (ent.CurrentPlayerOwner == myPlayer.Id)
        {
            if (ent.CurrentLevel >= ent.MaxLevel)
                return 0;
            else
            {
                objective = ent.ExpForNextLevel - ent.CurrentExp;
            }
        }
        if ((ent.CurrentPlayerOwner == GlobalData.NO_PLAYER && ent.CurrentContestantId == myPlayer.Id) || (ent.CurrentPlayerOwner == GlobalData.NO_PLAYER && ent.CurrentContestantId == GlobalData.NO_PLAYER))
        {
            objective = ent.ExpForNextLevel - ent.CurrentExp;
        }
        else if (ent.CurrentPlayerOwner == GlobalData.NO_PLAYER && ent.CurrentContestantId != myPlayer.Id)
        {
            objective = ent.ExpForNextLevel + ent.CurrentHealth;
        }
        else
        {
            if (turnNeutral)
                objective = ent.CurrentUnits + ent.CurrentHealth;
            else
                objective = ent.CurrentHealth + ent.CurrentUnits + ent.GetExpForLEvel(0);
        }

        if (randomNumber)
            return Mathf.RoundToInt(objective * (1.4f - Random.value));
        else
            return objective;
    }
}
