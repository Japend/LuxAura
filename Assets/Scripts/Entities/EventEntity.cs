using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EventEntity : ClockEventReceiver {

    protected const int EXP_FOR_LEVEL_1 = 100;
    protected static Vector3 SCALE_LEVEL_1 = Vector3.one;
    protected const int EXP_FOR_LEVEL_2 = 150;
    protected static Vector3 SCALE_LEVEL_2 = new Vector3(1.3f, 1.3f, 1.3f);
    protected const int EXP_FOR_LEVEL_3 = 200;
    protected static Vector3 SCALE_LEVEL_3 = new Vector3(1.7f, 1.7f, 1.7f);


    protected bool conquerable;

    protected int unitsToConquer;
    protected int maxHealth;
    public int MaxHealth { get { return maxHealth; } }

    protected int currentHealth;
    public int CurrentHealth {  get { return currentHealth; } }

    protected int maxLevel;
    public int MaxLevel { get { return maxLevel; } }

    protected int expForNextLevel;
    public int ExpForNextLevel { get { return expForNextLevel; } }

    protected int currentExp;
    public int CurrentExp { get { return currentExp; } }

    protected int currentLevel;
    public int CurrentLevel { get {return currentLevel;} }

    protected int currentPlayerOwner;
    public int CurrentPlayerOwner {  get { return currentPlayerOwner; } }
    
    protected int selectedUnits;
    public int SelectedUnits { get { return selectedUnits; } }

    protected int currentUnits;
    public int CurrentUnits { get { return currentUnits; } }

    protected int currentContestantId;
    public int CurrentContestantId { get { return currentContestantId; } }

    protected float generationRatio;

    //stores its position in the planets array and serves as an id
    protected int id = int.MinValue;
    public int Id {
        get { return id; }
        set
        {
            if (id != int.MinValue)
                Debug.LogError("SE INTENTO CAMBIAR LA ID DE UN PLANETA");
            else
                id = value;
        }
    }

    /// <summary>
    /// Sets the parameters for the entity
    /// </summary>
    /// <param name="player">The player that owns th entity</param>
    /// <param name="conquerable">If it can be conquered</param>
    /// <param name="uToConquer">Necessary units to conquer</param>
    /// <param name="maxLevel">Max entity level</param>
    /// <param name="genRatio">Units generation ratio per second</param>
    public virtual void SetParameters(int player = GlobalData.NO_PLAYER,  int maxLevel = 3, bool conquerable = true, int uToConquer = 100, int genRatio = 1)
    {
        currentPlayerOwner = player;
        this.conquerable = conquerable;
        this.unitsToConquer = uToConquer;
        this.maxLevel = maxLevel;
        currentHealth = maxHealth;
        currentLevel = 0;
        currentUnits = 0;
        currentExp = 0;
        print(gameObject.name + " " + currentPlayerOwner);
    }

    public virtual void Start()
    {
        print("BASE AWAKE EVENT ENTITY");
        Clock.Instance.AddListener(this);
    }

    public virtual void OnDestroy()
    {
        Clock.Instance.RemoveListener(this);
    }

    public abstract void SufferAttack(AttackInfo info);

    public int SelectUnits(bool selectAll = false)
    {
        int result;

        if (currentUnits <= 0)
            return 0;
        else if (currentUnits <= 5)
        {
            selectedUnits += 1;
            currentUnits--;
            return 1;
        }

        if (selectAll)
        {
            selectedUnits = currentUnits;
            result = currentUnits;
            currentUnits = 0;
        }
        else
        {
            selectedUnits += currentUnits / 3;
            result = currentUnits / 3;
            currentUnits -= result;
        }
        return result;
    }

    public void DeselectUnits()
    {
        currentUnits += selectedUnits;
        selectedUnits = 0;
    }

    public void UseUnits(GameObject objective)
    {
        AttackInfo info = new AttackInfo(gameObject, objective, currentPlayerOwner, selectedUnits);
        AttackPool.Instance.Attack(info);
        selectedUnits = 0;
    }

    /// <summary>
    /// Uses selected or non selected units
    /// </summary>
    /// <param name="info"></param>
    /// <param name="nonSelected">if true, no selected units will be used</param>
    public void UseUnits(AttackInfo info, bool nonSelected = false)
    {
        if (info.units > currentUnits)
            info.units = currentUnits;

        AttackPool.Instance.Attack(info);
        selectedUnits = 0;
        if (nonSelected)
            currentUnits -= info.units;
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

    public Vector3 getScaleForLevel(int lvl)
    {
        switch (lvl)
        {
            default:
            case 0:
                return SCALE_LEVEL_1;
            case 1:
                return SCALE_LEVEL_2;
            case 2:
                return SCALE_LEVEL_3;
        }
    }


    /// <summary>
    /// Returns a simplified class with the same exact state of this instance
    /// This state is also saved in the return class as an snapshot
    /// </summary>
    /// <returns></returns>
    public TEventEntity TakeSnapshot()
    {
        return new TPlanet(currentUnits, maxHealth, currentHealth, expForNextLevel, currentLevel, transform.position, currentPlayerOwner, maxLevel, currentExp, currentContestantId, id);
    }
}
