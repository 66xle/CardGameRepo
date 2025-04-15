using UnityEngine;

public class CameraSystem : MonoBehaviour
{
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
        Avatar avatarPlayingCard = moveToPosGA.avatarPlayingCard;
        Avatar avatarOpponent = moveToPosGA.avatarOpponent;
        CombatStateMachine ctx = moveToPosGA.ctx;

        if (avatarPlayingCard is Player)
        {
            ctx.followCam.LookAt = null;
            ctx.followCam.transform.rotation = ctx.defaultCam.transform.rotation;
            ctx.followCam.LookAt = avatarOpponent.transform;
            ctx.panCam.LookAt = avatarOpponent.transform;
            ctx.followCam.Priority = 30;
        }
    }

    private void ReturnToPosReactionPre(ReturnToPosGA returnToPosGA)
    {
        if (returnToPosGA.avatarPlayingCard is Player)
        {
            returnToPosGA.ctx.panCam.Priority = 0;
        }
    }

    private void ReturnToPosReactionPost(ReturnToPosGA returnToPosGA)
    {
        if (returnToPosGA.avatarPlayingCard is Player)
        {
            returnToPosGA.ctx.followCam.Priority = 10;
        }
    }

    private void TriggerAttackAnimReaction(TriggerAttackAnimGA triggerAttackAnimGA)
    {
        Avatar avatarPlayingCard = triggerAttackAnimGA.avatarPlayingCard;
        CombatStateMachine ctx = triggerAttackAnimGA.ctx;

        if (avatarPlayingCard is Player)
        {
            ctx.panCam.transform.position = ctx.followCam.transform.position;
            ctx.panCam.transform.rotation = ctx.followCam.transform.rotation;
            ctx.panCam.Priority = 31;
        }
    }
}
