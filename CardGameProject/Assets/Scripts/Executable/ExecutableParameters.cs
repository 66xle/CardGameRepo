using UnityEngine;
using System;
using System.Collections.Generic;

public static class ExecutableParameters
{
    public static CombatStateMachine ctx;
    public static Avatar avatarPlayingCard;
    public static Avatar avatarOpponent;
    public static Card card;
    public static WeaponData weapon;

    public static List<Avatar> Targets;
    public static List<Avatar> Queue;
    public static CardTarget CardTarget;
}
