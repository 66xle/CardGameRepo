using UnityEngine;

public class CounterGA : GameAction
{
    public Avatar AvatarOpponent;
    public Animator AvatarPlayingCardController;
    public Animator OpponentController;

    public CounterGA(Avatar avatarOpponent, Animator avatarPlayingCardController, Animator opponentController)
    {
        AvatarOpponent = avatarOpponent;
        AvatarPlayingCardController = avatarPlayingCardController;
        OpponentController = opponentController;
    }
}
