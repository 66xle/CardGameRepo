using DG.Tweening;
using UnityEngine;

public class MoveToPosGA : GameAction
{
    public Avatar AvatarPlayingCard;
    public Avatar AvatarOpponent;
    public bool MoveToCenter;
    public float DistanceOffset;

    public MoveToPosGA(Avatar avatarToMove, Avatar avatarOpponent, bool moveToCenter, float distanceOffset)
    {
        AvatarPlayingCard = avatarToMove;
        AvatarOpponent = avatarOpponent;
        MoveToCenter = moveToCenter;
        DistanceOffset = distanceOffset;
    }
}
