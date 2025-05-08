using MyBox;
using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    [MustBeAssigned] public CombatStateMachine Ctx;

    private void OnEnable()
    {
        Debug.Log($"CameraSystem enabled with ID {GetInstanceID()}, Ctx = {Ctx}");

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
            Ctx.followCam.LookAt = null;
            Ctx.followCam.transform.rotation = Ctx.defaultCam.transform.rotation;
            Ctx.followCam.LookAt = avatarOpponent.transform;
            Ctx.panCam.LookAt = avatarOpponent.transform;
            Ctx.followCam.Priority = 30;
        }
    }

    private void ReturnToPosReactionPre(ReturnToPosGA returnToPosGA)
    {
        if (returnToPosGA.AvatarPlayingCard is Player)
        {
            Ctx.panCam.Priority = 0;
        }
    }

    private void ReturnToPosReactionPost(ReturnToPosGA returnToPosGA)
    {
        if (returnToPosGA.AvatarPlayingCard is Player)
        {
            Ctx.followCam.Priority = 10;
        }
    }

    private void TriggerAttackAnimReaction(TriggerAttackAnimGA triggerAttackAnimGA)
    {
        Avatar avatarPlayingCard = triggerAttackAnimGA.AvatarPlayingCard;

        if (avatarPlayingCard is Player)
        {
            Ctx.panCam.transform.position = Ctx.followCam.transform.position;
            Ctx.panCam.transform.rotation = Ctx.followCam.transform.rotation;
            Ctx.panCam.Priority = 31;
        }
    }
}
