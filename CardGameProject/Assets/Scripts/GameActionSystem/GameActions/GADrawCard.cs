using UnityEngine;

public class GADrawCard : GameAction
{
    public Avatar Avatar;
    public int DrawAmount;

    public GADrawCard(Avatar avatar, int drawAmount)
    {
        Avatar = avatar;
        DrawAmount = drawAmount;
    }
}
