using Cinemachine;
using MyBox;
using UnityEngine;
using UnityEngine.Playables;

public class CameraSystem : MonoBehaviour
{
    [MustBeAssigned] [SerializeField] CameraManager cm;

    private void OnEnable()
    {
        ActionSystem.SubscribeReaction<MoveToPosGA>(MoveToPosReactionPre, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<ReturnToPosGA>(ReturnToPosReactionPre, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<ReturnToPosGA>(ReturnToPosReactionPost, ReactionTiming.POST);
        ActionSystem.SubscribeReaction<TriggerAttackAnimGA>(TriggerAttackAnimReaction, ReactionTiming.PRE);
    }

    private void OnDisable()
    {
        ActionSystem.UnsubscribeReaction<MoveToPosGA>(MoveToPosReactionPre, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<ReturnToPosGA>(ReturnToPosReactionPre, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<ReturnToPosGA>(ReturnToPosReactionPost, ReactionTiming.POST);
        ActionSystem.UnsubscribeReaction<TriggerAttackAnimGA>(TriggerAttackAnimReaction, ReactionTiming.PRE);
    }


    private void MoveToPosReactionPre(MoveToPosGA moveToPosGA)
    {
        if (moveToPosGA.FollowTimeline != null)
        {
            cm.SetTimeline(moveToPosGA.FollowTimeline);
            cm.ToggleDirector(true);
            return;
        }

        cm.FollowState();
    }

    private void ReturnToPosReactionPre(ReturnToPosGA returnToPosGA)
    {
        //cm.FollowBackState();
    }

    private void ReturnToPosReactionPost(ReturnToPosGA returnToPosGA)
    {
        cm.DefaultState();
    }

    private void TriggerAttackAnimReaction(TriggerAttackAnimGA triggerAttackAnimGA)
    {
        if (!triggerAttackAnimGA.EnableAttackCamera) return;

        if (triggerAttackAnimGA.AttackTimeline != null)
        {
            cm.SetTimeline(triggerAttackAnimGA.AttackTimeline);
            cm.ToggleDirector(true);
            return;
        }

        cm.AttackState();
    }
}
