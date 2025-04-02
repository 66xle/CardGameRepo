using UnityEngine;

public class ReturnToPosGA : GameAction
{
    public Avatar avatarPlayingCard;
    public CombatStateMachine ctx;

    public bool IsReturnFinished;

    public ReturnToPosGA(Avatar avatarToMove, CombatStateMachine ctx)
    {
        this.avatarPlayingCard = avatarToMove;
        this.ctx = ctx;

        IsReturnFinished = false;
    }
}
