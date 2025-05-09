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
        bool moveToCenter = moveToPosGA.MoveToCenter;

        Animator avatarPlayingCardController = avatarPlayingCard.GetComponent<Animator>();
        avatarPlayingCardController.SetTrigger("Move");

        // Determine position to move to
        Transform currentTransform = avatarPlayingCard.transform;
        Transform opponentTransform = moveToCenter ? avatarOpponent.transform.parent.parent : avatarOpponent.transform;

        Vector3 currentPos = new Vector3(currentTransform.position.x, 0, currentTransform.position.z);
        Vector3 opponentPos = new Vector3(opponentTransform.position.x, 0, opponentTransform.position.z);

        Vector3 posToMove = opponentPos + opponentTransform.transform.forward * 1.5f;

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
        Avatar avatarPlayingCard = triggerAttackAnimGA.AvatarPlayingCard;
        Animator animator = avatarPlayingCard.GetComponent<Animator>();

        Debug.Log(triggerAttackAnimGA.WeaponType.ToString());

        animator.SetTrigger(triggerAttackAnimGA.WeaponType.ToString());
        animator.SetBool("IsAttacking", true);

        yield return null;
    }
}
