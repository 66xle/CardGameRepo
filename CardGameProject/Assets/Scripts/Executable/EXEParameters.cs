using System.Collections.Generic;

public static class EXEParameters
{
    public static CombatStateMachine Ctx;
    public static Avatar AvatarPlayingCard;
    public static Avatar AvatarOpponent;
    public static CardRuntime CardRuntime;
    public static WeaponData WeaponData;

    public static List<Avatar> Targets;
    public static List<Avatar> Queue;
    public static CardTarget CardTarget;
}
