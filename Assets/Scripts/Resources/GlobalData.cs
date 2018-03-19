using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalData
{
    #region ACTIONS
    public enum ACTIONS
    {
        AttackOther, AttackSelf, Wait
    }
    #endregion

    #region COLORS
    public static Color NEUTRAL_COLOR = new Vector4(0.2f, 0.2f, 0.2f, 1f);
    public static Color PLAYER_COLOR = new Vector4(0.2f, 0.2f, 0.7f, 1f);
    public static Color ENEMY_1_COLOR = new Vector4(0.9176f, 0.3176f, 0.0f, 1f);
    public static Color ENEMY_2_COLOR = new Vector4(0.4274f, 0.91764f, 0.1686f, 1f);
    public static Color ENEMY_3_COLOR = new Vector4(0.898039f, 0f, 0.729411f, 1f);
    #endregion

    public static Color GetColor(int playerId)
    {
        switch (playerId)
        {
            case GlobalData.NO_PLAYER:
                return NEUTRAL_COLOR;
            case GlobalData.HUMAN_PLAYER:
                return PLAYER_COLOR;
            case 1:
                return ENEMY_1_COLOR;
            case 2:
                return ENEMY_2_COLOR;
            case 3:
                return ENEMY_3_COLOR;
            default:
                return new Color(0, 0, 0, 1);
        }
    }

    #region CONSTANTS
    public const double MILISECONDS_BETWEEN_TICKS = 200;
    public const double MILISECONDS_BETWEEN__AI_TICKS = 4000;
    public const int TIE = -2;
    public const int NO_PLAYER = -1;
    public const int HUMAN_PLAYER = 0;
    public const int AI_PLAYERS = 1;
    public const int NUMBER_OF_ACTIONS = 5;
    public const int MONTECARLO_TIMER_MILISECONDS = 2500;

    public const int MONTECARLO_REWARD = 10;
    public const int MONTECARLO_PENALIZATION = -10;
    #endregion
}
