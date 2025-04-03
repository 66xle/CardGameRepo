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

        avatarToTakeDamage.TakeDamage(ExecutableParameters.card.value);
        avatarToTakeDamage.GetComponent<Animator>().SetTrigger("TakeDamage");

        if (avatarToTakeDamage.IsGuardReducible(ExecutableParameters.weapon.type))
            avatarToTakeDamage.ReduceGuard();


        if (avatarToTakeDamage.IsAvatarDead())
        {
            avatarToTakeDamage.GetComponent<Animator>().SetTrigger("Death");
        }
        else if (avatarToTakeDamage.isGuardBroken())
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
