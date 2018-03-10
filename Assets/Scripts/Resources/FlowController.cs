using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowController : MonoBehaviour {

    private static FlowController instance;
    public static FlowController Instance
    {
        get { return FlowController.Instance; }
    }

    public GameObject VictoryText;
    private Game currentGame;

    private float remainingTime;
    private M_FlowController test;
    private bool paused;

    void Awake()
    {
        instance = this;
        GameObject aux = GameObject.Find("Level");
        EventEntity[] planets = new EventEntity[aux.transform.childCount];
        for (int i = 0; i < aux.transform.childCount; i++)
        {
            planets[i] = aux.transform.GetChild(i).GetComponent<EventEntity>();
            planets[i].Id = i;
        }

        currentGame = new Game(planets);

        remainingTime = 5f;
        test = new M_FlowController();
    }
	
	// Update is called once per frame
	void Update () {
        if (currentGame.SomeoneWon() != -1)
        {
            Clock.Instance.OnApplicationQuit();
            VictoryText.SetActive(true);
        }

        remainingTime -= Time.deltaTime;
        if (remainingTime <= 0)
        {
            remainingTime = 5;
            //test.StartTraining(currentGame.GetSnapshot());
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            Pause_Continue();
        }
	}

    private void Pause_Continue()
    {
        if (paused)
        {

            paused = false;
            Clock.Instance.Continue();
            Time.timeScale = 1f;
        }
        else
        {
            //DebugSnapshot();
            TestAdvanceTurnAndExecuteAction();
            paused = true;
            Clock.Instance.Stop();
            Time.timeScale = 0f;
        }
    }

    #region DEBUG
#if UNITY_EDITOR
    private TGame DebugSnapshot(TGame g = null)
    {
        TGame snapshot;
        if (g == null)
        {
            print("TAKING SNAPSHOT....");
            snapshot = currentGame.GetSnapshot();
        }
        else
            snapshot = g;

        print("DEBUGGING PLANETS");
        foreach (TPlanet pl in snapshot.Planets)
        {
            print("Planet " + pl.Id + " located in " + pl.Position + " belongs to " + pl.CurrentPlayerOwner);
            print("This planet has (health/exp/units) " + pl.CurrentHealth + "/" + pl.CurrentExp + "/" + pl.CurrentUnits);
            print("It is in level " + pl.CurrentLevel + " of a miaxximum of " + pl.MaxLevel + " and needs " + (pl.ExpForNextLevel - pl.CurrentExp) + "/" + pl.ExpForNextLevel + " to level up");
        }

        print("DEBUGGING PLAYERS");
        foreach (TPlayer pl in snapshot.Players)
        {
            print("Player " + pl.Id + " has a total of " + pl.Planets.Count + " planets under its domain");
            print("This planets are:");
            foreach (TPlanet plan in pl.Planets)
            {
                print("Planet " + plan.Id);
            }
        }
        print("DEBUGGING ATTACKS");
        foreach(TAttackInfo att in snapshot.PendingAttacks)
        {
            print("Attack from player " + att.Player + " to planet " + att.Destiny + " with " + att.Units);
            print("Will reach objective in " + att.remainingTurns);
        }
        return snapshot;
    }

    private void TestAdvanceTurnAndExecuteAction()
    {
        TGame game = DebugSnapshot();
        print("");
        M_FlowController test = new M_FlowController();
        test.TESTAdvanceTurnAndExecuteActions(0, Utilities.Actions.Wait, game);
        DebugSnapshot(game);
    }
#endif
    #endregion
}
