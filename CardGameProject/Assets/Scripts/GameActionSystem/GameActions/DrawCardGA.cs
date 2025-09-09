using UnityEngine;

public class DrawCardGA : GameAction
{
    public Avatar Avatar;
    public int DrawAmount;

    public DrawCardGA(Avatar avatar, int drawAmount)
    {
        Avatar = avatar;
        DrawAmount = drawAmount;
    }
}
