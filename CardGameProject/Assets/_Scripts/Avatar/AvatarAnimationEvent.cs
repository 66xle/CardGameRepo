using UnityEngine;
using UnityEngine.VFX;

public class AvatarAnimationEvent : MonoBehaviour
{
    [SerializeField] Avatar _avatar;

    public void AnimationEventAttack()
    {
        _avatar.DoDamage = true;
    }

    public void AnimationEventAttackFinish()
    {
        _avatar.IsAttackFinished = true;
        _avatar.Animator.SetBool("IsAttacking", false);
    }

    public void AnimationEventPlaySound()
    {
        AudioManager.Instance.PlayAudioResource();
    }

    public void AnimationEventDisableRecoil()
    {
        AnimationEventAttackFinish();
        _avatar.IsRecoilDone = true;
        _avatar.Animator.SetBool("IsRecoiled", false);
    }

    public void EnableWeaponTrail()
    {
        VisualEffect weaponTrail = _avatar.RightHolder.GetComponentInChildren<VisualEffect>();
        weaponTrail.Play();

        Debug.Log("WEAPON TRAIL");
    }

    public void DisableWeaponTrail()
    {
        VisualEffect weaponTrail = _avatar.RightHolder.GetComponentInChildren<VisualEffect>();
        weaponTrail.enabled = false;
    }
}
