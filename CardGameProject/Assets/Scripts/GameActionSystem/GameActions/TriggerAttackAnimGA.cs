using UnityEngine;

public class TriggerAttackAnimGA : GameAction
{
    public Avatar avatarPlayingCard;
    public CombatStateMachine ctx;

    public TriggerAttackAnimGA(Avatar avatarPlayingCard, CombatStateMachine ctx)
    {
        this.avatarPlayingCard = avatarPlayingCard;
        this.ctx = ctx;
    }
}
