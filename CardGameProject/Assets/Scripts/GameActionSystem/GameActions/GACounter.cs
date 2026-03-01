using UnityEngine;

public class GACounter : GameAction
{
    public Avatar AvatarOpponent;
    public Avatar AvatarPlayingCard;
    public Animator OpponentController;
    public Animator AvatarPlayingCardController;

    public GACounter(Avatar avatarOpponent, Avatar avatarPlayingCard)
    {
        AvatarOpponent = avatarOpponent;
        AvatarPlayingCard = avatarPlayingCard;
        OpponentController = avatarOpponent.Animator;
        AvatarPlayingCardController = avatarPlayingCard.Animator;
    }
}
