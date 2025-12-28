using Cinemachine;
using MyBox;
using UnityEngine;
using UnityEngine.Playables;

public class GASystemCamera : MonoBehaviour
{
    [MustBeAssigned] [SerializeField] CameraManager cm;

    private void OnEnable()
    {
        ActionSystem.SubscribeReaction<GAMoveToPos>(MoveToPosReactionPre, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<GAReturnToPos>(ReturnToPosReactionPre, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<GAReturnToPos>(ReturnToPosReactionPost, ReactionTiming.POST);
        ActionSystem.SubscribeReaction<GATriggerAttackAnim>(TriggerAttackAnimReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<GATriggerAnim>(TriggerAnimReaction, ReactionTiming.PRE);
    }

    private void OnDisable()
    {
        ActionSystem.UnsubscribeReaction<GAMoveToPos>(MoveToPosReactionPre, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<GAReturnToPos>(ReturnToPosReactionPre, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<GAReturnToPos>(ReturnToPosReactionPost, ReactionTiming.POST);
        ActionSystem.UnsubscribeReaction<GATriggerAttackAnim>(TriggerAttackAnimReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<GATriggerAnim>(TriggerAnimReaction, ReactionTiming.PRE);
    }


    private void MoveToPosReactionPre(GAMoveToPos moveToPosGA)
    {
        if (moveToPosGA.FollowTimeline != null)
        {
            cm.SpawnTimeline(moveToPosGA.FollowTimeline);
            return;
        }

        cm.FollowState();
    }

    private void ReturnToPosReactionPre(GAReturnToPos returnToPosGA)
    {
        //cm.FollowBackState();
    }

    private void ReturnToPosReactionPost(GAReturnToPos returnToPosGA)
    {
        cm.DestroyTimeline();


        cm.DefaultState();
    }

    private void TriggerAttackAnimReaction(GATriggerAttackAnim triggerAttackAnimGA)
    {
        if (!triggerAttackAnimGA.IsAttackAnimation) return;

        if (triggerAttackAnimGA.AttackTimeline != null)
        {
            cm.SpawnTimeline(triggerAttackAnimGA.AttackTimeline);
            return;
        }

        cm.AttackState();
    }

    private void TriggerAnimReaction(GATriggerAnim TriggerAnimGA)
    {
        if (TriggerAnimGA.AnimTimeline != null)
        {
            cm.SpawnTimeline(TriggerAnimGA.AnimTimeline);
            return;
        }
    }
}
