using System.Collections;
using MyBox;
using UnityEngine;

public class DamageSystem : MonoBehaviour
{
    [MustBeAssigned] public CombatStateMachine Ctx;
    

    private void OnEnable()
    {
        ActionSystem.AttachPerformer<TakeDamageFromWeaponGA>(TakeDamageFromWeaponPerformer);
        ActionSystem.AttachPerformer<TakeGuardDamageGA>(TakeGuardDamagePerformer);
        ActionSystem.AttachPerformer<CounterGA>(CounterPerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<TakeDamageFromWeaponGA>();
        ActionSystem.DetachPerformer<TakeGuardDamageGA>();
        ActionSystem.DetachPerformer<CounterGA>();
    }

    private IEnumerator TakeDamageFromWeaponPerformer(TakeDamageFromWeaponGA takeDamageFromWeaponGA)
    {
        Avatar avatarToTakeDamage = takeDamageFromWeaponGA.AvatarToTakeDamage;

        avatarToTakeDamage.TakeDamage(takeDamageFromWeaponGA.Damage);

        if (takeDamageFromWeaponGA.CardTarget != CardTarget.Self)
        {
            avatarToTakeDamage.IsTakeDamage = true;
            avatarToTakeDamage.GetComponent<Animator>().SetTrigger("TakeDamage");

            if (avatarToTakeDamage.IsGuardReducible(takeDamageFromWeaponGA.DamageType))
                avatarToTakeDamage.ReduceGuard(1);
        }


        if (avatarToTakeDamage.IsAvatarDead())
        {
            avatarToTakeDamage.GetComponent<Animator>().SetTrigger("Death");
            avatarToTakeDamage.DictReactiveEffects.Clear();
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

        avatarToTakeDamage.UpdateStatsUI();

        yield return null;
    }

    private IEnumerator TakeGuardDamagePerformer(TakeGuardDamageGA takeGuardDamageGA)
    {
        Avatar avatarToTakeDamage = takeGuardDamageGA.AvatarToTakeDamage;

        avatarToTakeDamage.ReduceGuard(takeGuardDamageGA.GuardDamage);

        if (takeGuardDamageGA.CardTarget != CardTarget.Self)
        {
            avatarToTakeDamage.IsTakeDamage = true;
            avatarToTakeDamage.GetComponent<Animator>().SetTrigger("TakeDamage");
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

    private IEnumerator CounterPerformer(CounterGA counterGA)
    {
        counterGA.OpponentController.SetBool("isReady", false);
        counterGA.OpponentController.SetTrigger("Counter");

        counterGA.AvatarPlayingCardController.SetTrigger("Recoil");

        counterGA.AvatarOpponent.IsInCounterState = false;



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
