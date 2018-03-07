using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Planet : EventEntity
{
    public const int MAX_PLANET_HEALTH = 100;

    private const float SLIDER_UPDATE_VELOCITY = 3f;
    private const float COLOR_FADING_TIME = 5f;
    private const float SCALE_ADJUSTING_TIME = 5f;


    public Text UnitsNumberText;
    public Material Material;
    public Slider healthSlider, expSlider;
    int frameCount = 0;
    bool setHealthToMaxPending = true;
    MeshRenderer renderer;

    // Use this for initialization
    public override void Start()
    {
        base.Start();
        currentUnits = 0;
        print("PLANET AWAKE");

        healthSlider.value = healthSlider.maxValue;
        healthSlider.enabled = false;

        maxHealth = MAX_PLANET_HEALTH;
        currentHealth = MaxHealth;
        expForNextLevel = EXP_FOR_LEVEL_1;
        currentLevel = 0;
        renderer = this.gameObject.GetComponentInChildren<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        UnitsNumberText.text = System.Convert.ToString(CurrentUnits);

        frameCount++;
        if (frameCount >= 100)
        {
            checkOwner();
            frameCount = 0;
        }
    }

    //Checks if the color is correct and if its not, updates it
    private void checkColor()
    {
        if (renderer.material.color != (Color) GlobalData.GetColor(currentPlayerOwner))
        {
            StopCoroutine("FadeColor");
            StartCoroutine(FadeColor(currentPlayerOwner));
        }
        
    }

    /// <summary>
    /// Checks if the player belongs to a player. If it does, makes sure everything is on order. If not,
    /// doesn't do anything. If it belongs to two, resets it
    /// </summary>
    private void checkOwner()
    {

        for (int j = 0; j < Game.Instance.Players.Length; j++)
        {
            if (Game.Instance.Players[j] == this || Game.Instance.Players[j].Planets == null)
                continue;
            //if two players own it, we reset the player
            if(Game.Instance.Players[j].Planets.Contains(this) && j != currentPlayerOwner)
            {
                Game.Instance.Players[j].Planets.Remove(this);
                Game.Instance.Players[currentPlayerOwner].Planets.Remove(this);
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

        checkColor();
    }

    public override void Tick(Clock.EventType type)
    {
        switch(type)
        {
            case Clock.EventType.MainTick:
            default:
                if(CurrentPlayerOwner > GlobalData.NO_PLAYER)
                    currentUnits += 1 + currentLevel;
                break;

            case Clock.EventType.IATick:
                break;
    }
    }

    public override void SetParameters(int player = GlobalData.NO_PLAYER, int maxLevel = 3, bool conquerable = true, int uToConquer = 100, int genRatio = 1)
    {
        base.SetParameters(player, maxLevel, conquerable, uToConquer, genRatio);
        gameObject.GetComponentInChildren<MeshRenderer>().material = Material;
        gameObject.GetComponentInChildren<MeshRenderer>().material.color = GlobalData.GetColor(player);
    }


    public override void SufferAttack(AttackInfo info)
    {
        if (setHealthToMaxPending)
        {
            StopCoroutine("UpdateHealthSliderUp");
            StopCoroutine("UpdateHealthSliderDown");
            currentHealth = maxHealth;
            healthSlider.value = healthSlider.maxValue;
            healthSlider.enabled = false;
            setHealthToMaxPending = false;
        }
        //if the planet is neutral
        if (CurrentPlayerOwner == GlobalData.NO_PLAYER)
        {
            if (CurrentContestantId == info.player)
            {
                //if the planet is conquered
                if (info.units >= currentHealth)
                {
                    if (currentPlayerOwner != GlobalData.NO_PLAYER)
                        Game.Instance.Players[currentPlayerOwner].Planets.Remove(this);

                    currentUnits += info.units - currentHealth;
                    currentPlayerOwner = info.player;
                    currentHealth = 0;
                    StopCoroutine("FadeColor");
                    StartCoroutine(FadeColor(info.player));
                    currentContestantId = -1;
                    Game.Instance.Players[currentPlayerOwner].Planets.Add(this);
                    setHealthToMaxPending = true;

                }
                else
                    currentHealth -= info.units;

                StopCoroutine("UpdateHealthSliderUp");
                StopCoroutine("UpdateHealthSliderDown");
                StartCoroutine("UpdateHealthSliderDown");
            }

            else
            {
                if (info.units > MaxHealth - currentHealth)
                {
                    info.units -= MaxHealth - currentHealth;
                    currentHealth = MaxHealth;
                    currentContestantId = info.player;
                    SufferAttack(info);
                }
                else
                {
                    currentHealth += info.units;
                }
                StopCoroutine("UpdateHealthSliderUp");
                StopCoroutine("UpdateHealthSliderDown");
                StartCoroutine("UpdateHealthSliderUp");
            }

        }

        else
        {
            if (info.player == currentPlayerOwner)
            {
                attackFromSelf(info);
            }
            else
            {
                attackFromOther(info);
            }
        }

    }

    private void attackFromSelf(AttackInfo info)
    {
        if (currentHealth < MaxHealth)
        {
            if (info.units < MaxHealth - currentHealth)
            {
                currentHealth += info.units;
            }
            else
            {
                info.units -= MaxHealth - currentHealth;
                currentHealth = MaxHealth;
                attackFromSelf(info);
            }
            StopCoroutine("UpdateHealthSliderDown");
            StopCoroutine("UpdateHealthSliderUp");
            StartCoroutine("UpdateHealthSliderUp");
        }

        else if (currentLevel >= maxLevel)
        {
            currentUnits += info.units;
        }
        else
        {
            if (info.units < ExpForNextLevel - CurrentExp)
            {
               currentExp += info.units;
            }
            else
            {
                info.units -= ExpForNextLevel - CurrentExp;
                currentExp = ExpForNextLevel;
                currentUnits += info.units;
                levelUp();
            }

            StopCoroutine("UpdateExpSliderUp");
            StopCoroutine("UpdateExpSliderDown");
            StartCoroutine("UpdateExpSliderUp");
        }

    }


    private void attackFromOther(AttackInfo info)
    {
        //if the planet is conquered
        if (info.units >= CurrentUnits + currentHealth + CurrentExp)
        {
            info.units -= CurrentUnits + currentHealth + CurrentExp;
            currentUnits = 0;
            currentUnits += info.units;
            Game.Instance.Players[currentPlayerOwner].Planets.Remove(this);
            currentPlayerOwner = info.player;
            Game.Instance.Players[currentPlayerOwner].Planets.Add(this);
            currentHealth = 0;
            currentExp = 0;
            setHealthToMaxPending = true;
            StopCoroutine("FadeColor");
            StartCoroutine(FadeColor(currentPlayerOwner));
            levelDown();

            StopCoroutine("UpdateHealthSlider");
            StartCoroutine("UpdateHealthSliderDown");
            StopCoroutine("UpdateExpSliderDown");
            StopCoroutine("UpdateExpSliderUp");
            StartCoroutine("UpdateExpSliderDown");
        }
        //if its health or exp is reduced
        else if (info.units > CurrentUnits)
        {
            info.units -= CurrentUnits;
            currentUnits = 0;

            if (currentExp > 0)
            {
                if (info.units > CurrentExp)
                {
                    info.units -= CurrentExp;
                    currentExp = 0;
                    attackFromOther(info);
                }
                else
                {
                    currentExp -= info.units;
                }

                StopCoroutine("UpdateExpSliderDown");
                StopCoroutine("UpdateExpSliderUp");
                StartCoroutine("UpdateExpSliderDown");
            }
            else
            {
                currentHealth -= info.units;
                StopCoroutine("UpdateHealthSliderDown");
                StopCoroutine("UpdateHealthSliderUp");
                StartCoroutine("UpdateHealthSliderDown");
            }
        }
        else //if no damage to the planet
        {
            currentUnits -= info.units;
        }


    }

    private void levelUp()
    {
        currentLevel++;
        StopCoroutine("ScaleLevel");
        StartCoroutine("ScaleLevel");
    }

    private void levelDown()
    {
        currentLevel = 0;
        StopCoroutine("ScaleLevel");
        StartCoroutine("ScaleLevel");
    }


    IEnumerator ScaleLevel()
    {
        Vector3 currentScale = transform.localScale;
        Vector3 objectiveScale = getScaleForLevel(currentLevel);
        float initialTime = Time.time;

        while (transform.localScale != objectiveScale)
        {
            transform.localScale = Vector3.Lerp(currentScale, objectiveScale, (Time.time - initialTime) / SCALE_ADJUSTING_TIME);
            yield return null;
        }
    }

    IEnumerator FadeColor(int ownerId)
    {
        MeshRenderer rend = gameObject.GetComponentInChildren<MeshRenderer>();
        Color initialColor = rend.material.color;
        Color objectiveColor = GlobalData.GetColor(currentPlayerOwner);
        float initialTime = Time.time;

        while (rend.material.color != objectiveColor)
        {
            rend.material.color = Vector4.Lerp(initialColor, objectiveColor, (Time.time - initialTime) / COLOR_FADING_TIME);
            yield return null;
        }

        yield return null;
    }

    IEnumerator UpdateHealthSliderDown()
    {
        float initialValue = healthSlider.value;
        float finalValue = currentHealth * 100 / MaxHealth;
        float initialTime = Time.time;

        healthSlider.enabled = true;

        while (healthSlider.value > finalValue)
        {
            healthSlider.value -= (Time.time - initialTime) * SLIDER_UPDATE_VELOCITY;
            yield return null;
        }

        healthSlider.value = finalValue;

        if (finalValue <= 0)
        {
            healthSlider.enabled = false;
            currentHealth = MaxHealth;
            setHealthToMaxPending = false;
        }
    }


    IEnumerator UpdateHealthSliderUp()
    {
        float initialValue = healthSlider.value;
        float finalValue = currentHealth * 100 / MaxHealth;
        float initialTime = Time.time;

        healthSlider.enabled = true;
        while (healthSlider.value < finalValue)
        {
            healthSlider.value += (Time.time - initialTime) * SLIDER_UPDATE_VELOCITY;
            yield return null;
        }

        healthSlider.value = finalValue;

        if (finalValue == MaxHealth)
        {
            healthSlider.enabled = false;
        }
    }


    IEnumerator UpdateExpSliderUp()
    {
        float initialValue = expSlider.value;
        float finalValue = CurrentExp * 100 / ExpForNextLevel;
        float initialTime = Time.time;

        expSlider.enabled = true;

        while (expSlider.value < finalValue)
        {
            expSlider.value += (Time.time - initialTime) * SLIDER_UPDATE_VELOCITY;
            yield return null;
        }

        expSlider.value = finalValue;

        if (expSlider.value == expSlider.maxValue)
        {
            currentExp = 0;
            expSlider.value = 0;
            expSlider.enabled = false;
            expForNextLevel = GetExpForLEvel(currentLevel);
        }
    }


    IEnumerator UpdateExpSliderDown()
    {
        float initialValue = expSlider.value;
        float finalValue = CurrentExp * 100 / ExpForNextLevel;
        float initialTime = Time.time;

        expSlider.enabled = true;
        while (expSlider.value > finalValue)
        {
            expSlider.value -= (Time.time - initialTime) * SLIDER_UPDATE_VELOCITY;
            yield return null;
        }

        expSlider.value = finalValue;

        if (expSlider.value == 0)
        {
            expSlider.enabled = false;
            expForNextLevel = GetExpForLEvel(currentLevel);
        }
    }
}
