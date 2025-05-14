using DG.Tweening;
using MyBox;
using System.Collections;
using System.Collections.Generic;
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

        string animationName = GetAttackAnimation(triggerAttackAnimGA.AnimationList);

        string category = GetWeaponCategory(animationName);
        animator.SetTrigger(category);

        string attackType = GetAttackType(animationName);
        animator.SetTrigger(attackType);

        int index = GetAnimationIndex(animationName);
        animator.SetInteger("AnimationIndex", index);

        Debug.Log($"{animationName} - {category} - {attackType} - {index}");

        animator.SetBool("IsAttacking", true);

        yield return null;
    }

    private string GetAttackAnimation(List<string> list)
    {
        int index = Random.Range(0, list.Count);

        return list[index];
    }

    private string GetWeaponCategory(string name)
    {
        if (name.Contains("Long"))
        {
            return "Long";
        }
        else if (name.Contains("Short"))
        {
            return "Short";
        }

        return "General";
    }

    private string GetAttackType(string name)
    {
        if (name.Contains(AttackType.Strike.ToString()))
        {
            return "Strike";
        }
        else if (name.Contains(AttackType.Heavy.ToString()))
        {
            return "Heavy";
        }
        else if (name.Contains(AttackType.AOE.ToString()))
        {
            return "AOE";
        }

        Debug.LogError("Animation Clip missing attack type! " + name);
        return "Strike";
    }

    private int GetAnimationIndex(string name)
    {
        char index = name.ToCharArray()[name.Length - 1];

        return int.Parse(index.ToString());
    }
}
