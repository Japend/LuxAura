using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class RandomAI : AI {


    public Actions Decide()
    {
        return (Actions)Random.Range(0, 5);
    }
}
