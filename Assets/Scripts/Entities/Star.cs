using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : EventEntity {


    // Use this for initialization
    public override void Start()
    {
        base.Start();
        print("STAR AWAKE");
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void Tick(Clock.EventType type)
    {
 	    print("Tick de planeta " + type);
    }

    public override void SufferAttack(AttackInfo info)
    {
        throw new System.NotImplementedException();
    }
}
