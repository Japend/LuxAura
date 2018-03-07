using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OLD_TEventEntity
{

    protected const int EXP_FOR_LEVEL_1 = 100;
    protected const int EXP_FOR_LEVEL_2 = 150;
    protected const int EXP_FOR_LEVEL_3 = 200;

    #region SNAPSHOT VARIABLES
    /// <summary>
    /// This variables are used for saving and restoring the state of the object
    /// By doing that this way, we don't need to crerate new objects, thus speeding up the process
    /// ONLY THE VARIABLES THAT CAN CHANGE DURING A GAME ARE STORED
    /// </summary>

    private int s_maxHealth;
    private int s_currentHealth;
    private int s_expForNextLevel;
    private int s_currentExp;
    private int s_currentLevel;
    private int s_currentPlayerOwner;
    private int s_currentUnits;
    private int s_currentContestantId;
    #endregion


    protected int maxHealth;
    public int MaxHealth { get { return maxHealth; } }

    protected int currentHealth;
    public int CurrentHealth { get { return currentHealth; } }

    protected int maxLevel;
    public int MaxLevel { get { return maxLevel; } }

    protected int expForNextLevel;
    public int ExpForNextLevel { get { return expForNextLevel; } }

    protected int currentExp;
    public int CurrentExp { get { return currentExp; } }

    protected int currentLevel;
    public int CurrentLevel { get { return currentLevel; } }

    protected int currentPlayerOwner;
    public int CurrentPlayerOwner { get { return currentPlayerOwner; } }

    protected int currentUnits;
    public int CurrentUnits { get { return currentUnits; } }

    protected int currentContestantId;
    public int CurrentContestantId { get { return currentContestantId; } }

    protected Vector3 position;
    public Vector3 Position { get { return position; } }

    protected int[] turnsToReachOtherPlanets;
    public int[] TurnsToReachOtherPlanets { get { return turnsToReachOtherPlanets; } }

    protected TGame currentGame;

    //stores its position in the planets array and serves as an id
    protected int id;
    public int Id { get { return id; } }


    public abstract void Tick(int turns = 1);


    /// <summary>
    /// Sets the parameters for the entity
    /// </summary>
    /// <param name="player">The player that owns th entity</param>
    /// <param name="maxLevel">Max entity level</param>
    /// <param name="genRatio">Units generation ratio per second</param>
    public virtual void SetParameters(int player = GlobalData.NO_PLAYER, int maxLevel = 3, int genRatio = 1)
    {
        currentPlayerOwner = player;
        this.maxLevel = maxLevel;
        currentHealth = maxHealth;
        currentLevel = 0;
        currentUnits = 0;
        currentExp = 0;
    }


    public abstract void SufferAttack(TAttackInfo info);


    public void UpdateDistances(int[] dist)
    {
        turnsToReachOtherPlanets = dist;
    }


    /// <summary>
    /// Checks if it has enough units and if not, corrects the attack
    /// 
    /// </summary>
    /// <param name="info"></param>
    public void UseUnits(TAttackInfo info)
    {
        if (info.Units > currentUnits)
            info.Units = currentUnits;
        currentUnits -= info.Units;
        OLD_M_FlowController.Instance.CurrentGame.AddAttack(info);
    }

    public int GetExpForLEvel(int lvl)
    {
        switch (lvl)
        {
            default:
            case 0:
                return EXP_FOR_LEVEL_1;
            case 1:
                return EXP_FOR_LEVEL_2;
            case 2:
                return EXP_FOR_LEVEL_3;
        }
    }

    /// <summary>
    /// Saves the current state of the object to recover it later
    /// </summary>
    public void TakeSnapshot()
    {
        s_currentContestantId = currentContestantId;
        s_currentExp = currentExp;
        s_currentHealth = currentHealth;
        s_currentLevel = currentLevel;
        s_currentPlayerOwner = currentPlayerOwner;
        s_expForNextLevel = expForNextLevel;
        s_maxHealth = maxHealth;
        s_currentUnits = currentUnits;
    }

    /// <summary>
    /// Restores the saved state of this object
    /// </summary>
    public void RestoreSnapshot()
    {
        currentContestantId = s_currentContestantId;
        currentExp = s_currentExp;
        currentHealth = s_currentHealth;
        currentLevel = s_currentLevel;
        currentPlayerOwner = s_currentPlayerOwner;
        expForNextLevel = s_expForNextLevel;
        maxHealth = s_maxHealth;
        currentUnits = s_currentUnits;
    }
}
