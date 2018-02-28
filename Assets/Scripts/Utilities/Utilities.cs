using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    public struct Pair<T, U>
    {
        public T first;
        public U second;

        public Pair(T frst, U scnd)
        {
            first = frst;
            second = scnd;
        }
    }

    public static class Utilities
    {
        public static int GetDistanceInTurns(Vector3 ori, Vector3 dest)
        {
            return (int)(Vector3.Distance(ori, dest) / Attack.MOVEMENT_SPEED * (1000f / GlobalData.MILISECONDS_BETWEEN_TICKS));
        }
    }
}
