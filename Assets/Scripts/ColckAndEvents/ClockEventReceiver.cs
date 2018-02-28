using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ClockEventReceiver : MonoBehaviour {

    public abstract void Tick(Clock.EventType type);
}
