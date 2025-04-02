using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    private void OnEnable()
    {
        ActionSystem.SubscribeReaction<MoveToPosGA>(MoveToPosReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<ReturnToPosGA>(ReturnToPosReaction, ReactionTiming.PRE);
    }

    private void OnDisable()
    {
        ActionSystem.UnsubscribeReaction<MoveToPosGA>(MoveToPosReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<ReturnToPosGA>(ReturnToPosReaction, ReactionTiming.PRE);
    }

    private void MoveToPosReaction(MoveToPosGA moveToPosGA)
    {
        Avatar avatarPlayingCard = moveToPosGA.avatarPlayingCard;
        Avatar avatarOpponent = moveToPosGA.avatarOpponent;
        CombatStateMachine ctx = moveToPosGA.ctx;

        if (avatarPlayingCard.gameObject.CompareTag("Player"))
        {
            ctx.followCam.LookAt = null;
            ctx.followCam.transform.rotation = ctx.defaultCam.transform.rotation;
            ctx.followCam.LookAt = avatarOpponent.transform;
            ctx.panCam.LookAt = avatarOpponent.transform;
            ctx.followCam.Priority = 30;
        }
    }

    private void ReturnToPosReaction(ReturnToPosGA returnToPosGA)
    {
        if (returnToPosGA.avatarPlayingCard.gameObject.CompareTag("Player"))
            returnToPosGA.ctx.panCam.Priority = 0;
    }
}
