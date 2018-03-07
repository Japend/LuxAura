using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class DumbAI : AI {

    public Actions Decide()
    {
        return Actions.Wait;
    }

}
