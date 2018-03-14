using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class RandomAI : AI {

    System.Random rand;

    public RandomAI()
    {
        rand = new System.Random();
    }

    public Actions Decide()
    {
        return (Actions)rand.Next(0, 6);
    }
}
