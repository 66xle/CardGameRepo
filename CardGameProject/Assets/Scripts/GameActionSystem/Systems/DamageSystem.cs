using System.Collections;
using UnityEngine;

public class DamageSystem : MonoBehaviour
{
    private void OnEnable()
    {
        ActionSystem.AttachPerformer<TakeDamageFromWeaponGA>(TakeDamageFromWeaponPerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<TakeDamageFromWeaponGA>();
    }

    private IEnumerator TakeDamageFromWeaponPerformer(TakeDamageFromWeaponGA takeDamageFromWeaponGA)
    {
        foreach (Avatar avatarToTakeDamage in takeDamageFromWeaponGA.targets)
        {
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
        }

        yield return null;
    }
}
