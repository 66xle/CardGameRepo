using UnityEngine;

public class CounterGA : GameAction
{
    public Avatar AvatarOpponent;
    public Avatar AvatarPlayingCard;
    public Animator OpponentController;
    public Animator AvatarPlayingCardController;

    public CounterGA(Avatar avatarOpponent, Avatar avatarPlayingCard)
    {
        AvatarOpponent = avatarOpponent;
        AvatarPlayingCard = avatarPlayingCard;
        OpponentController = avatarOpponent.GetComponent<Animator>();
        AvatarPlayingCardController = avatarPlayingCard.GetComponent<Animator>();
    }
}
