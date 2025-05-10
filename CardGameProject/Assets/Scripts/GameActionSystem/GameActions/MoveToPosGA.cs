using DG.Tweening;
using UnityEngine;

public class MoveToPosGA : GameAction
{
    public Avatar AvatarPlayingCard;
    public Avatar AvatarOpponent;
    public bool MoveToCenter;

    public MoveToPosGA(Avatar avatarToMove, Avatar avatarOpponent, bool moveToCenter)
    {
        AvatarPlayingCard = avatarToMove;
        AvatarOpponent = avatarOpponent;
        MoveToCenter = moveToCenter;
    }
}
