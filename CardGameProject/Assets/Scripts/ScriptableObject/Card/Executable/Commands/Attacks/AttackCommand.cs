using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackCommand : TargetCommand
{
    protected abstract override List<Avatar> GetTargets();

    public override IEnumerator Execute(Action<bool> IsConditionTrue)
    {
        CombatStateMachine ctx = ExecutableParameters.ctx;
        Avatar avatarPlayingCard = ExecutableParameters.avatarPlayingCard;
        Avatar avatarOpponent = ExecutableParameters.avatarOpponent;

        Animator avatarPlayingCardController = avatarPlayingCard.GetComponent<Animator>();
        Animator opponentController = avatarOpponent.GetComponent<Animator>();

        Debug.Log(ActionSystem.Instance.IsPerforming);

        if (avatarOpponent.isInCounterState)
        {
            CounterGA counterGA = new(avatarOpponent, avatarPlayingCardController, opponentController);
            ActionSystem.Instance.Perform(counterGA);
        }
        else
        {
            List<Avatar> targets = GetTargets();

            foreach (Avatar avatarToTakeDamage in targets)
            {
                TakeDamageFromWeaponGA takeDamageFromWeaponGA = new(avatarToTakeDamage, ctx, ExecutableParameters.card.value, ExecutableParameters.weapon.type);
                ActionSystem.Instance.Perform(takeDamageFromWeaponGA);

                SpawnDamageUIPopupGA spawnDamageUIPopupGA = new(takeDamageFromWeaponGA.ctx.combatUIManager, takeDamageFromWeaponGA.avatarToTakeDamage, takeDamageFromWeaponGA.damage, Color.white);
                takeDamageFromWeaponGA.PostReactions.Add(spawnDamageUIPopupGA);
            }
        }

        yield return null;
    }
}
