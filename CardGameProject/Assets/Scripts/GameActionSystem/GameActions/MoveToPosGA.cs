using DG.Tweening;
using UnityEngine;

public class MoveToPosGA : GameAction
{
    public Avatar AvatarPlayingCard;
    public Avatar AvatarOpponent;


    public MoveToPosGA(Avatar avatarToMove, Avatar avatarOpponent)
    {
        AvatarPlayingCard = avatarToMove;
        AvatarOpponent = avatarOpponent;
    }
}
