using MyBox;
using UnityEngine;

public class OverrideRootMotion : MonoBehaviour
{
    [SerializeField] Animator Animator;
    [SerializeField] Avatar _avatar;

    private void OnAnimatorMove()
    {
        if (!_avatar.AllowRootMotion)
            return;

        // When taking damage don't apply root motion
        if (Animator.IsInTransition(0) && Animator.GetNextAnimatorStateInfo(0).IsName("Take Damage") ||
            Animator.GetCurrentAnimatorStateInfo(0).IsName("Take Damage")) return;

        // Apply root motion manually
        _avatar.transform.position += _avatar.Animator.deltaPosition;
        _avatar.transform.rotation *= _avatar.Animator.deltaRotation;
    }
}
