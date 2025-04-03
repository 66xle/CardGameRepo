using UnityEngine;

public class CounterGA : GameAction
{
    public Avatar avatarOpponent;
    public Animator avatarPlayingCardController;
    public Animator opponentController;

    public CounterGA(Avatar avatarOpponent, Animator avatarPlayingCardController, Animator opponentController)
    {
        this.avatarOpponent = avatarOpponent;
        this.avatarPlayingCardController = avatarPlayingCardController;
        this.opponentController = opponentController;
    }
}
