using Cinemachine;
using MyBox;
using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    [MustBeAssigned] [SerializeField] CameraManager cm;

    private void OnEnable()
    {
        ActionSystem.SubscribeReaction<MoveToPosGA>(MoveToPosReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<ReturnToPosGA>(ReturnToPosReactionPre, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<ReturnToPosGA>(ReturnToPosReactionPost, ReactionTiming.POST);
        ActionSystem.SubscribeReaction<TriggerAttackAnimGA>(TriggerAttackAnimReaction, ReactionTiming.PRE);
    }

    private void OnDisable()
    {
        ActionSystem.UnsubscribeReaction<MoveToPosGA>(MoveToPosReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<ReturnToPosGA>(ReturnToPosReactionPre, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<ReturnToPosGA>(ReturnToPosReactionPost, ReactionTiming.POST);
        ActionSystem.UnsubscribeReaction<TriggerAttackAnimGA>(TriggerAttackAnimReaction, ReactionTiming.PRE);
    }


    private void MoveToPosReaction(MoveToPosGA moveToPosGA)
    {
        Avatar avatarPlayingCard = moveToPosGA.AvatarPlayingCard;
        Avatar avatarOpponent = moveToPosGA.AvatarOpponent;

        if (avatarPlayingCard is Player)
        {
            cm.FollowState();
        }
    }

    private void ReturnToPosReactionPre(ReturnToPosGA returnToPosGA)
    {
        if (returnToPosGA.AvatarPlayingCard is Player)
        {
            //cm.FollowBackState();
        }
    }

    private void ReturnToPosReactionPost(ReturnToPosGA returnToPosGA)
    {
        if (returnToPosGA.AvatarPlayingCard is Player)
        {
            cm.DefaultState();
        }
    }

    private void TriggerAttackAnimReaction(TriggerAttackAnimGA triggerAttackAnimGA)
    {
        Avatar avatarPlayingCard = triggerAttackAnimGA.AvatarPlayingCard;

        if (avatarPlayingCard is Player)
        {
            cm.AttackState();
        }
    }
}
