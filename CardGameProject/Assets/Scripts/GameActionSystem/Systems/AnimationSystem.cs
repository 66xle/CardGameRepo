using DG.Tweening;
using System.Collections;
using UnityEngine;

public class AnimationSystem : MonoBehaviour
{
    private void OnEnable()
    {
        ActionSystem.AttachPerformer<MoveToPosGA>(MoveToPosPerformer);
        ActionSystem.AttachPerformer<ReturnToPosGA>(ReturnToPosPerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<MoveToPosGA>();
        ActionSystem.DetachPerformer<ReturnToPosGA>();
    }

    private IEnumerator MoveToPosPerformer(MoveToPosGA moveToPosGA)
    {
        Avatar avatarPlayingCard = moveToPosGA.avatarPlayingCard;
        Avatar avatarOpponent = moveToPosGA.avatarOpponent;
        CombatStateMachine ctx = moveToPosGA.ctx;

        Animator avatarPlayingCardController = avatarPlayingCard.GetComponent<Animator>();
        avatarPlayingCardController.SetTrigger("Move");

        // Determine position to move to
        Transform currentTransform = avatarPlayingCard.transform;
        Transform opponentTransform = avatarOpponent.transform;

        Vector3 currentPos = new Vector3(currentTransform.position.x, 0, currentTransform.position.z);
        Vector3 opponentPos = new Vector3(opponentTransform.position.x, 0, opponentTransform.position.z);

        Vector3 posToMove = opponentPos + opponentTransform.parent.transform.forward * 1.5f;

        if (avatarPlayingCard.gameObject.CompareTag("Player"))
        {
            ctx.followCam.LookAt = null;
            ctx.followCam.transform.rotation = ctx.defaultCam.transform.rotation;
            ctx.followCam.LookAt = avatarOpponent.transform;
            ctx.panCam.LookAt = avatarOpponent.transform;
            ctx.followCam.Priority = 30;
        }

        Tween tween = currentTransform.DOMove(new Vector3(posToMove.x, currentTransform.position.y, posToMove.z), ctx.moveDuration).SetEase(ctx.moveAnimCurve);

        yield return tween.WaitForCompletion();

        moveToPosGA.IsMoveFinished = true;
    }

    private IEnumerator ReturnToPosPerformer(ReturnToPosGA returnToPosGA)
    {
        Avatar avatarPlayingCard = returnToPosGA.avatarPlayingCard;
        CombatStateMachine ctx = returnToPosGA.ctx;

        // Determine position to move to
        Transform currentTransform = avatarPlayingCard.transform;
        Transform parentTransform = currentTransform.parent.transform;

        Vector3 currentPos = new Vector3(currentTransform.position.x, 0, currentTransform.position.z);
        Vector3 parentPos = new Vector3(parentTransform.position.x, 0, parentTransform.position.z);

        Vector3 posToMove = parentPos;

        Tween tween = currentTransform.DOMove(new Vector3(posToMove.x, currentTransform.position.y, posToMove.z), ctx.jumpDuration).SetEase(ctx.jumpAnimCurve);

        yield return tween.WaitForCompletion();

        returnToPosGA.IsReturnFinished = true;
    }
}
