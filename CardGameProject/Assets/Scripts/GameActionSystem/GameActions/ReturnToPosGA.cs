using UnityEngine;

public class ReturnToPosGA : GameAction
{
    public Avatar AvatarPlayingCard;

    public bool IsReturnFinished;

    public ReturnToPosGA(Avatar avatarToMove)
    {
        AvatarPlayingCard = avatarToMove;

        IsReturnFinished = false;
    }
}
