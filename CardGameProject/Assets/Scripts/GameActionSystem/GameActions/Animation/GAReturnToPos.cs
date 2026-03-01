using UnityEngine;

public class GAReturnToPos : GameAction
{
    public Avatar AvatarPlayingCard;

    public bool IsReturnFinished;

    public GAReturnToPos(Avatar avatarToMove)
    {
        AvatarPlayingCard = avatarToMove;

        IsReturnFinished = false;
    }
}
