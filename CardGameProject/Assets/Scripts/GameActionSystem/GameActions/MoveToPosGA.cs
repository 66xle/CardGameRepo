using DG.Tweening;
using UnityEngine;

public class MoveToPosGA : GameAction
{
    public Avatar avatarPlayingCard;
    public Avatar avatarOpponent;
    public CombatStateMachine ctx;


    public MoveToPosGA(Avatar avatarToMove, Avatar avatarOpponent, CombatStateMachine ctx)
    {
        this.avatarPlayingCard = avatarToMove;
        this.avatarOpponent = avatarOpponent;
        this.ctx = ctx;
    }
}
