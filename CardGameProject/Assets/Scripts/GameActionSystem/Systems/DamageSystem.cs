using System.Collections;
using UnityEngine;

public class DamageSystem : MonoBehaviour
{
    private void OnEnable()
    {
        ActionSystem.AttachPerformer<TakeDamageFromWeaponGA>(TakeDamageFromWeaponPerformer);
        ActionSystem.AttachPerformer<CounterGA>(CounterPerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<TakeDamageFromWeaponGA>();
        ActionSystem.DetachPerformer<CounterGA>();
    }

    private IEnumerator TakeDamageFromWeaponPerformer(TakeDamageFromWeaponGA takeDamageFromWeaponGA)
    {
        Avatar avatarToTakeDamage = takeDamageFromWeaponGA.avatarToTakeDamage;

        avatarToTakeDamage.TakeDamage(takeDamageFromWeaponGA.damage);

        if (ExecutableParameters.CardTarget != CardTarget.Self)
        {
            avatarToTakeDamage.GetComponent<Animator>().SetTrigger("TakeDamage");

            if (avatarToTakeDamage.IsGuardReducible(takeDamageFromWeaponGA.damageType))
                avatarToTakeDamage.ReduceGuard();
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
                //ReduceHitToRecover();
            }
            else
            {
                takeDamageFromWeaponGA.ApplyGuardBroken(takeDamageFromWeaponGA.ctx, avatarToTakeDamage);
            }
        }

        avatarToTakeDamage.UpdateStatsUI();

        yield return null;
    }

    private IEnumerator CounterPerformer(CounterGA counterGA)
    {
        counterGA.opponentController.SetBool("isReady", false);
        counterGA.opponentController.SetTrigger("Counter");

        counterGA.avatarPlayingCardController.SetTrigger("Recoil");

        counterGA.avatarOpponent.isInCounterState = false;



        yield return null;
    }
}
