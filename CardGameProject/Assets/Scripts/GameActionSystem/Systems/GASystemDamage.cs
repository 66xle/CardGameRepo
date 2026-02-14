using System.Collections;
using MyBox;
using UnityEngine;

public class GASystemDamage : MonoBehaviour
{
    [MustBeAssigned] public CombatStateMachine Ctx;
    

    private void OnEnable()
    {
        ActionSystem.AttachPerformer<GATakeDamageFromWeapon>(TakeDamageFromWeaponPerformer);
        ActionSystem.AttachPerformer<GATakeGuardDamage>(TakeGuardDamagePerformer);
        ActionSystem.AttachPerformer<GACounter>(CounterPerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<GATakeDamageFromWeapon>();
        ActionSystem.DetachPerformer<GATakeGuardDamage>();
        ActionSystem.DetachPerformer<GACounter>();
    }

    private IEnumerator TakeDamageFromWeaponPerformer(GATakeDamageFromWeapon takeDamageFromWeaponGA)
    {
        Avatar avatarToTakeDamage = takeDamageFromWeaponGA.AvatarToTakeDamage;

        avatarToTakeDamage.TakeDamage(takeDamageFromWeaponGA.Damage);

        if (takeDamageFromWeaponGA.CardTarget != CardTarget.Self && !avatarToTakeDamage.IsInCounterState)
        {
            avatarToTakeDamage.IsHit = true;

            if (!avatarToTakeDamage.IsInCounterState)
                avatarToTakeDamage.Animator.SetTrigger("TakeDamage");

            if (avatarToTakeDamage.IsGuardReducible(takeDamageFromWeaponGA.DamageType))
                avatarToTakeDamage.ReduceGuard(1);
        }


        if (avatarToTakeDamage.IsAvatarDead())
        {
            avatarToTakeDamage.Animator.SetTrigger("Death");
            avatarToTakeDamage.DictReactiveEffects.Clear();
            avatarToTakeDamage.PlayDeathSound();
        }
        else if (avatarToTakeDamage.IsGuardBroken())
        {
            // Check if avatar has guard broken effect
            if (avatarToTakeDamage.hasStatusEffect(Effect.GuardBroken))
            {
                //ReduceHitToRecover(avatarToTakeDamage);
            }
            else
            {
                ApplyGuardBroken(avatarToTakeDamage);
            }
        }

        avatarToTakeDamage.PlayHurtSound();
        avatarToTakeDamage.UpdateStatsUI();

        yield return null;
    }

    private IEnumerator TakeGuardDamagePerformer(GATakeGuardDamage takeGuardDamageGA)
    {
        Avatar avatarToTakeDamage = takeGuardDamageGA.AvatarToTakeDamage;

        if (!avatarToTakeDamage.IsInCounterState)
            avatarToTakeDamage.ReduceGuard(takeGuardDamageGA.GuardDamage);

        if (takeGuardDamageGA.CardTarget != CardTarget.Self)
        {
            avatarToTakeDamage.IsHit = true;

            if (!avatarToTakeDamage.IsInCounterState)
                avatarToTakeDamage.Animator.SetTrigger("TakeDamage");
        }

        if (avatarToTakeDamage.IsGuardBroken())
        {
            // Check if avatar has guard broken effect
            if (avatarToTakeDamage.hasStatusEffect(Effect.GuardBroken))
            {
                //ReduceHitToRecover(avatarToTakeDamage);
            }
            else
            {
                ApplyGuardBroken(avatarToTakeDamage);
            }
        }

        avatarToTakeDamage.UpdateStatsUI();

        yield return null;
    }

    private IEnumerator CounterPerformer(GACounter counterGA)
    {
        //counterGA.OpponentController.SetBool("isReady", false);
        //counterGA.AvatarOpponent.IsInCounterState = false;
        counterGA.OpponentController.SetTrigger("Counter");
        counterGA.AvatarOpponent.IsHit = true;

        counterGA.AvatarPlayingCard.IsCountered = true;
        counterGA.AvatarPlayingCardController.SetBool("IsRecoiled", true);
        counterGA.AvatarPlayingCardController.SetTrigger("Recoil");


        yield return null;
    }

    public void ReduceHitToRecover(Avatar avatarOpponent)
    {
        for (int i = avatarOpponent.ListOfEffects.Count - 1; i >= 0; i--)
        {
            if (avatarOpponent.ListOfEffects[i].Effect != Effect.GuardBroken)
                continue;

            avatarOpponent.ListOfEffects[i].CurrentTurnsRemaning--;
            if (avatarOpponent.ListOfEffects[i].CurrentTurnsRemaning <= 0)
            {
                avatarOpponent.RecoverGuardBreak();
                avatarOpponent.ListOfEffects[i].OnRemoval(avatarOpponent);
                avatarOpponent.ListOfEffects.RemoveAt(i);
            }
        }
    }

    public void ApplyGuardBroken(Avatar avatarOpponent)
    {
        if (avatarOpponent.ArmourType == ArmourType.Light || avatarOpponent.ArmourType == ArmourType.None) avatarOpponent.ApplyStatusEffect(Ctx.GuardBreakLightArmourData.StatusEffect.Clone());
        else if (avatarOpponent.ArmourType == ArmourType.Medium) avatarOpponent.ApplyStatusEffect(Ctx.GuardBreakMediumArmourData.StatusEffect.Clone());
        else if (avatarOpponent.ArmourType == ArmourType.Heavy) avatarOpponent.ApplyStatusEffect(Ctx.GuardBreakHeavyArmourData.StatusEffect.Clone());
    }
}
