using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TAttackInfo
{
    public int remainingTurns;

    private int player;
    public int Player { get { return player; } }

    public int Units;

    private int destiny;
    public int Destiny{ get { return destiny; } }

    public TAttackInfo(int turns, int playerId, int units, int destiny)
    {
        remainingTurns = turns;
        player = playerId;
        this.Units = units;
        this.destiny = destiny;
    }

    public bool SubstractTurn()
    {
        return remainingTurns-- <= 0;
    }
}