using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TPlanet : TEventEntity
{
    public const int MAX_PLANET_HEALTH = 100;

    public TPlanet(int owner, Vector3 pos, int maxLevel, int id)
    {
        currentUnits = 0;
        maxHealth = MAX_PLANET_HEALTH;
        currentHealth = MaxHealth;
        expForNextLevel = EXP_FOR_LEVEL_1;
        currentLevel = 0;
        position = pos;
        currentPlayerOwner = owner;
        this.maxLevel = maxLevel;
        currentExp = 0;
        currentContestantId = GlobalData.NO_PLAYER;
        this.id = id;
    }

    //Use this constructor to instantiate in a certain state
    public TPlanet(int currentUnits, int maxHealth, int currentHealth, int expForNextLevel, int currentLevel, Vector3 pos, int ownerId, int maxLevel, int currentExp, int currentContestsant, int id)
    {
        this.currentUnits = currentUnits;
        this.maxHealth = maxHealth;
        this.currentHealth = currentHealth;
        this.expForNextLevel = expForNextLevel;
        this.currentLevel = currentLevel;
        position = pos;
        currentPlayerOwner = ownerId;
        this.maxLevel = maxLevel;
        this.currentExp = currentExp;
        this.currentContestantId = currentContestsant;
        this.id = id;
        TakeSnapshot();
    }


    /// <summary>
    /// Checks if the player belongs to a player. If it does, makes sure everything is on order. If not,
    /// doesn't do anything. If it belongs to two, resets it
    /// </summary>
  /* private void checkOwner()
    {

        for (int j = 0; j < Game.Instance.Players.Length; j++)
        {
            if (Game.Instance.Players[j] == this || Game.Instance.Players[j].Planets == null)
                continue;
            //if two players own it, we reset the player
            if (Game.Instance.Players[j].Planets.Contains(this) && j != currentPlayerOwner)
            {
                Game.Instance.Players[j].Planets.Remove(this);
                Game.Instance.Players[j].Planets.Remove(this);
                this.currentPlayerOwner = GlobalData.NO_PLAYER;
                this.currentUnits = 0;
                StopCoroutine("FadeColor");
                StartCoroutine(FadeColor(currentPlayerOwner));
                levelDown();
                currentHealth = maxHealth;
                expSlider.value = 0;
                expSlider.enabled = false;
                healthSlider.value = 100;
                healthSlider.enabled = false;
                Debug.LogError("Planet pertained to two different players");
                return;
            }
        }
    }*/

    public override void Tick(Clock.EventType type, int turns = 1)
    {
        switch (type)
        {
            case Clock.EventType.MainTick:
            default:
                if (CurrentPlayerOwner > GlobalData.NO_PLAYER && currentPlayerOwner != GlobalData.HUMAN_PLAYER)
                    currentUnits += (1 + currentLevel) * turns;
                break;

            case Clock.EventType.IATick:
                break;
        }
    }


    public override void SufferAttack(TAttackInfo info)
    {
        //if the planet is neutral
        if (CurrentPlayerOwner == GlobalData.NO_PLAYER)
        {
            //if the contestant is the same player
            if (CurrentContestantId == info.Player)
            {
                //if the planet is conquered
                if (info.Units >= currentHealth)
                { 
                    /*if (currentPlayerOwner != GlobalData.NO_PLAYER)
                        M_FlowController.Instance.Players[currentPlayerOwner].Planets.Remove(this);*/

                    currentUnits += info.Units - currentHealth;
                    currentPlayerOwner = info.Player;
                    currentHealth = maxHealth;
                    currentContestantId = -1;
                    M_FlowController.Instance.CurrentGame.Players[currentPlayerOwner].Planets.Add(this);

                }
                else
                    currentHealth -= info.Units;
            }

            else
            {
                //if the contestant is anoher player
                //if the attack surpases the other player attakc, becmoes the contestant (MaxHealth - curretn healt = oponent damage)
                if (info.Units > MaxHealth - currentHealth)
                {
                    info.Units -= MaxHealth - currentHealth;
                    currentHealth = MaxHealth;
                    currentContestantId = info.Player;
                    SufferAttack(info);
                }
                else
                {
                    currentHealth += info.Units;
                }
            }

        }

        else
        {
            if (info.Player == currentPlayerOwner)
            {
                attackFromSelf(info);
            }
            else
            {
                attackFromOther(info);
            }
        }

    }

    private void attackFromSelf(TAttackInfo info)
    {
        if (currentHealth < MaxHealth)
        {
            if (info.Units < MaxHealth - currentHealth)
            {
                currentHealth += info.Units;
            }
            else
            {
                info.Units -= MaxHealth - currentHealth;
                currentHealth = MaxHealth;
                attackFromSelf(info);
            }
        }

        else if (currentLevel >= maxLevel)
        {
            currentUnits += info.Units;
        }
        else
        {
            if (info.Units < ExpForNextLevel - CurrentExp)
            {
                currentExp += info.Units;
            }
            else
            {
                info.Units -= ExpForNextLevel - CurrentExp;
                currentExp = 0;
                currentUnits += info.Units;
                levelUp();
            }
        }
    }


    private void attackFromOther(TAttackInfo info)
    {
        //if the planet is conquered
        if (info.Units >= CurrentUnits + currentHealth + CurrentExp)
        {
            info.Units -= CurrentUnits + currentHealth + CurrentExp;
            currentUnits = 0;
            currentUnits += info.Units;
            M_FlowController.Instance.CurrentGame.Players[currentPlayerOwner].Planets.Remove(this);
            currentPlayerOwner = info.Player;
            M_FlowController.Instance.CurrentGame.Players[currentPlayerOwner].Planets.Add(this);
            currentHealth = maxHealth;
            currentExp = 0;
            levelDown();
        }
        //if its health or exp is reduced
        else if (info.Units > CurrentUnits)
        {
            info.Units -= CurrentUnits;
            currentUnits = 0;

            if (currentExp > 0)
            {
                if (info.Units > CurrentExp)
                {
                    info.Units -= CurrentExp;
                    currentExp = 0;
                    attackFromOther(info);
                }
                else
                {
                    currentExp -= info.Units;
                }
            }
            else
            {
                currentHealth -= info.Units;
            }
        }
        else //if no damage to the planet
        {
            currentUnits -= info.Units;
        }


    }

    private void levelUp()
    {
        currentLevel++;
    }

    private void levelDown()
    {
        currentLevel = 0;
    }
}
