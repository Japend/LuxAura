using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public abstract class BaseGame/* : MonoBehaviour*/{

    public Mutex GameMutex;
    protected List<TAttackInfo> pendingAttacks;

    protected BaseGame currentGame;
    public BaseGame CurrentGame { get { return currentGame; } }

}
