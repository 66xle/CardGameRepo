using UnityEngine;
using System;
using System.Collections.Generic;

public static class ExecutableParameters
{
    public static CombatStateMachine Ctx;
    public static Avatar AvatarPlayingCard;
    public static Avatar AvatarOpponent;
    public static Card Card;
    public static WeaponData WeaponData;

    public static List<Avatar> Targets;
    public static List<Avatar> Queue;
    public static CardTarget CardTarget;
}
