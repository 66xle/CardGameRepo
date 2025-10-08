using DG.Tweening;
using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSystem : MonoBehaviour
{
    [Header("Animation Settings")]
    public float moveDuration = 0.5f;
    public float jumpDuration = 0.5f;
    public AnimationCurve moveAnimCurve;
    public AnimationCurve jumpAnimCurve;

    private void OnEnable()
    {
        ActionSystem.AttachPerformer<MoveToPosGA>(MoveToPosPerformer);
        ActionSystem.AttachPerformer<ReturnToPosGA>(ReturnToPosPerformer);
        ActionSystem.AttachPerformer<TriggerAttackAnimGA>(TriggerAttackAnimPerformer);
        ActionSystem.AttachPerformer<TriggerAnimGA>(TriggerAnimPerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<MoveToPosGA>();
        ActionSystem.DetachPerformer<ReturnToPosGA>();
        ActionSystem.DetachPerformer<TriggerAttackAnimGA>();
        ActionSystem.DetachPerformer<TriggerAnimGA>();
    }

    private IEnumerator MoveToPosPerformer(MoveToPosGA moveToPosGA)
    {
        Avatar avatarPlayingCard = moveToPosGA.AvatarPlayingCard;
        Avatar avatarOpponent = moveToPosGA.AvatarOpponent;
        bool moveToCenter = moveToPosGA.MoveToCenter;

        float distanceOffset = moveToPosGA.DistanceOffset;

        Animator avatarPlayingCardController = avatarPlayingCard.GetComponent<Animator>();
        avatarPlayingCardController.SetTrigger("Move");

        // Determine position to move to
        Transform currentTransform = avatarPlayingCard.transform;
        Transform opponentTransform = moveToCenter ? avatarOpponent.transform.parent.parent : avatarOpponent.transform;

        Vector3 currentPos = new Vector3(currentTransform.position.x, 0, currentTransform.position.z);
        Vector3 opponentPos = new Vector3(opponentTransform.position.x, 0, opponentTransform.position.z);

        Vector3 dir = (currentPos - opponentPos).normalized;

        Vector3 posToMove = opponentPos + dir * (1.5f + distanceOffset);
        
        float tweenDuration = moveToPosGA.MoveTime > 0f ? moveToPosGA.MoveTime : moveDuration;
        Tween tween = currentTransform.DOMove(new Vector3(posToMove.x, currentTransform.position.y, posToMove.z), tweenDuration).SetEase(moveAnimCurve);

        Quaternion targetRotation = Quaternion.LookRotation(-dir);
        currentTransform.DORotate(targetRotation.eulerAngles, 1f, RotateMode.Fast); 

        yield return tween.WaitForCompletion();
    }

    private IEnumerator ReturnToPosPerformer(ReturnToPosGA returnToPosGA)
    {
        Avatar avatarPlayingCard = returnToPosGA.AvatarPlayingCard;

        Animator animator = avatarPlayingCard.GetComponent<Animator>();

        // Determine position to move to
        Transform currentTransform = avatarPlayingCard.transform;
        Transform parentTransform = currentTransform.parent.transform;

        Vector3 currentPos = new Vector3(currentTransform.position.x, 0, currentTransform.position.z);
        Vector3 parentPos = new Vector3(parentTransform.position.x, 0, parentTransform.position.z);

        Vector3 posToMove = parentPos;

        Tween tween = currentTransform.DOMove(new Vector3(posToMove.x, currentTransform.position.y, posToMove.z), jumpDuration).SetEase(jumpAnimCurve);

        yield return tween.WaitForCompletion();

        Quaternion targetRotation = Quaternion.LookRotation(Vector3.zero);
        currentTransform.DOLocalRotate(targetRotation.eulerAngles, 1f, RotateMode.Fast);


        returnToPosGA.IsReturnFinished = true;
    }

    private IEnumerator TriggerAttackAnimPerformer(TriggerAttackAnimGA triggerAttackAnimGA)
    {
        Avatar avatarPlayingCard = triggerAttackAnimGA.AvatarPlayingCard;
        Animator animator = avatarPlayingCard.GetComponent<Animator>();

        string animationName = triggerAttackAnimGA.AnimationName;

        string category = GetWeaponCategory(animationName);
        animator.SetTrigger(category);

        string attackType = GetAttackType(animationName);
        animator.SetTrigger(attackType);

        int index = GetAnimationIndex(animationName);
        animator.SetInteger("AnimationIndex", index);

        Debug.Log($"{animationName} - {category} - {attackType} - {index}");

        animator.SetBool("IsAttacking", true);

        AudioManager.Instance.SetAudioType(triggerAttackAnimGA.AudioType);

        yield return null;
    }

    private IEnumerator TriggerAnimPerformer(TriggerAnimGA triggerAnimGA)
    {
        Avatar avatarPlayingCard = triggerAnimGA.AvatarPlayingCard;
        Animator animator = avatarPlayingCard.GetComponent<Animator>();

        string animationName = triggerAnimGA.AnimationName;
        animator.CrossFade(animationName, 0.25f);

        AudioManager.Instance.SetAudioType(triggerAnimGA.AudioType);

        yield return null;
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
        if (name.Contains("Strike"))
        {
            return "Strike";
        }
        else if (name.Contains("Heavy"))
        {
            return "Heavy";
        }
        else if (name.Contains("AOE"))
        {
            return "AOE";
        }

        Debug.LogError($"Animation Clip missing attack type: '{name}'");
        return "Strike";
    }

    private int GetAnimationIndex(string name)
    {
        char index = name.ToCharArray()[name.Length - 1];

        return int.Parse(index.ToString());
    }
}
