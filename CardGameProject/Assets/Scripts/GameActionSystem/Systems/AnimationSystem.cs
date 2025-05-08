using DG.Tweening;
using MyBox;
using System.Collections;
using UnityEngine;

public class AnimationSystem : MonoBehaviour
{
    [MustBeAssigned] public CombatStateMachine Ctx;

    private void OnEnable()
    {
        ActionSystem.AttachPerformer<MoveToPosGA>(MoveToPosPerformer);
        ActionSystem.AttachPerformer<ReturnToPosGA>(ReturnToPosPerformer);
        ActionSystem.AttachPerformer<TriggerAttackAnimGA>(TriggerAttackAnimPerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<MoveToPosGA>();
        ActionSystem.DetachPerformer<ReturnToPosGA>();
        ActionSystem.DetachPerformer<TriggerAttackAnimGA>();
    }

    private IEnumerator MoveToPosPerformer(MoveToPosGA moveToPosGA)
    {
        Avatar avatarPlayingCard = moveToPosGA.AvatarPlayingCard;
        Avatar avatarOpponent = moveToPosGA.AvatarOpponent;

        Animator avatarPlayingCardController = avatarPlayingCard.GetComponent<Animator>();
        avatarPlayingCardController.SetTrigger("Move");

        // Determine position to move to
        Transform currentTransform = avatarPlayingCard.transform;
        Transform opponentTransform = avatarOpponent.transform;

        Vector3 currentPos = new Vector3(currentTransform.position.x, 0, currentTransform.position.z);
        Vector3 opponentPos = new Vector3(opponentTransform.position.x, 0, opponentTransform.position.z);

        Vector3 posToMove = opponentPos + opponentTransform.parent.transform.forward * 1.5f;

        Tween tween = currentTransform.DOMove(new Vector3(posToMove.x, currentTransform.position.y, posToMove.z), Ctx.moveDuration).SetEase(Ctx.moveAnimCurve);

        yield return tween.WaitForCompletion();
    }

    private IEnumerator ReturnToPosPerformer(ReturnToPosGA returnToPosGA)
    {
        Avatar avatarPlayingCard = returnToPosGA.AvatarPlayingCard;

        // Determine position to move to
        Transform currentTransform = avatarPlayingCard.transform;
        Transform parentTransform = currentTransform.parent.transform;

        Vector3 currentPos = new Vector3(currentTransform.position.x, 0, currentTransform.position.z);
        Vector3 parentPos = new Vector3(parentTransform.position.x, 0, parentTransform.position.z);

        Vector3 posToMove = parentPos;

        Tween tween = currentTransform.DOMove(new Vector3(posToMove.x, currentTransform.position.y, posToMove.z), Ctx.jumpDuration).SetEase(Ctx.jumpAnimCurve);

        yield return tween.WaitForCompletion();

        returnToPosGA.IsReturnFinished = true;
    }

    private IEnumerator TriggerAttackAnimPerformer(TriggerAttackAnimGA triggerAttackAnimGA)
    {
        triggerAttackAnimGA.AvatarPlayingCard.GetComponent<Animator>().SetTrigger("Attack");

        yield return null;
    }
}
