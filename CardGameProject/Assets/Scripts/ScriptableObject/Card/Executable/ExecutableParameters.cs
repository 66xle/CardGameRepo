using UnityEngine;
using System;

public static class ExecutableParameters
{
    public static CombatStateMachine ctx;
    public static Avatar avatarPlayingCard;
    public static Avatar avatarOpponent;
    public static Card card;
    public static WeaponData weapon;


    public static bool hasAttacked;
    public static bool hasMoved;


    public static Action MoveToEnemy;
    public static Action ReturnToPosition;

    public static void Clear()
    {
        avatarPlayingCard = null;
        avatarOpponent = null;
    }
}
