using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Timers;
using Utilities;
public class Clock : MonoBehaviour
{

    public enum EventType
    {
        MainTick,
        IATick
    }

    private static Clock instance;
    public static Clock Instance
    {
        get { return Clock.instance; }
    }

    Timer mainClock, AIclock;
    long ticks, AITicks;

    //listener variables
    ClockEventReceiver[] ticksReceivers;
    int currentReceivers;

    // Use this for initialization
    void Awake()
    {

        ticks = 0;

        mainClock = new Timer(GlobalData.MILISECONDS_BETWEEN_TICKS);
        mainClock.Elapsed += OnMainTick;
        mainClock.AutoReset = true;
        mainClock.Start();

        AIclock = new Timer(GlobalData.MILISECONDS_BETWEEN__AI_TICKS);
        AIclock.Elapsed += OnAITick;
        AIclock.AutoReset = true;
        AIclock.Start();

        if (instance == null)
            instance = this;
    }

    public void OnApplicationQuit()
    {
        print("Stoping clock");
        mainClock.Stop();
        mainClock.Dispose();
        AIclock.Stop();
        AIclock.Dispose();
    }

    public void Stop()
    {
        mainClock.Stop();
        AIclock.Stop();
    }
    public void Continue()
    {
        mainClock.Start();
        AIclock.Start();
    }

    private void OnMainTick(object sender, ElapsedEventArgs e)
    {
        ticks++;
        //print("TICK at " + System.DateTime.Now);
        System.Threading.Thread thread = new System.Threading.Thread(() => TickBubble(Clock.EventType.MainTick));
        thread.Start();
    }

    private void OnAITick(object sender, ElapsedEventArgs e)
    {
        AITicks++;
        System.Threading.Thread thread = new System.Threading.Thread(() => TickBubble(Clock.EventType.IATick));
        thread.Start();
    }

    /// <summary>
    /// Adds a event receiver to the clock so it is called when the clock ticks
    /// </summary>
    /// <param name="ear">Receiver to be added</param>
    public void AddListener(ClockEventReceiver ear)
    {
        if (ticksReceivers == null)
        {
            ticksReceivers = new ClockEventReceiver[10];
            currentReceivers = 0;
        }

        if (currentReceivers >= ticksReceivers.Length - 1)
            increaseReceiversCapacity();

        ticksReceivers[currentReceivers] = ear;
        currentReceivers++;
    }

    /// <summary>
    /// Removes the listener so it wont be called aggain
    /// </summary>
    /// <param name="ear">Listener to be removed</param>
    /// <param name="compact">If is false, the array wont be compacted. use this when multiple eliminations have to be done</param>
    /// <returns>True if it was removed, false it it wasn't found</returns>
    public bool RemoveListener(ClockEventReceiver ear)
    {
        for (int i = 0; i < ticksReceivers.Length; i++)
        {
            ClockEventReceiver aux;

            if (ticksReceivers[i] == ear)
            {
                //we swap with the last one and delete
                if (i < currentReceivers)
                {
                    aux = ticksReceivers[currentReceivers];
                    ticksReceivers[currentReceivers] = null;
                    ticksReceivers[i] = aux;
                }
                else
                    ticksReceivers[i] = null;
                currentReceivers--;

                return true;
            }
        }
        return false;
    }


    public void AddTimerForMontecarlo(System.Timers.ElapsedEventHandler receiver)
    {
        Timer mTimer = new Timer(GlobalData.MILISECONDS_BETWEEN_TICKS);
        mTimer.Elapsed += receiver;
        mTimer.AutoReset = true;
        mTimer.Enabled = true;
    }

    /// <summary>
    /// Increasses capacity of the clocks receiver array
    /// </summary>
    private void increaseReceiversCapacity()
    {
        int goalCapacity = ticksReceivers.Length * 2;
        ClockEventReceiver[] aux = new ClockEventReceiver[goalCapacity];

        for (int i = 0; i < goalCapacity; i++)
        {
            if (i < ticksReceivers.Length - 1)
                aux[i] = ticksReceivers[i];
            else
                aux[i] = null;
        }

        ticksReceivers = aux;
    }

    /// <summary>
    /// Spreads the event to all the listeners
    /// </summary>
    private void TickBubble(EventType eventType)
    {
        foreach (ClockEventReceiver rec in ticksReceivers)
        {
            rec.Tick(eventType);
        }
    }
}
