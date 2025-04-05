using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackCommand : ActionSequence
{

    public override IEnumerator Execute(Action<bool> IsConditionTrue)
    {
        CombatStateMachine ctx = ExecutableParameters.ctx;
        Avatar avatarPlayingCard = ExecutableParameters.avatarPlayingCard;
        Avatar avatarOpponent = ExecutableParameters.avatarOpponent;

        Animator avatarPlayingCardController = avatarPlayingCard.GetComponent<Animator>();
        Animator opponentController = avatarOpponent.GetComponent<Animator>();

        if (avatarOpponent.isInCounterState)
        {
            CounterGA counterGA = new(avatarOpponent, avatarPlayingCardController, opponentController);
            ActionSystem.Instance.AddGameActionToQueue(counterGA);
        }
        else
        {
            foreach (Avatar avatarToTakeDamage in ExecutableParameters.Targets)
            {
                TakeDamageFromWeaponGA takeDamageFromWeaponGA = new(avatarToTakeDamage, ctx, ExecutableParameters.card.value, ExecutableParameters.weapon.type);
                ActionSystem.Instance.AddGameActionToQueue(takeDamageFromWeaponGA);

                SpawnDamageUIPopupGA spawnDamageUIPopupGA = new(takeDamageFromWeaponGA.ctx.combatUIManager, takeDamageFromWeaponGA.avatarToTakeDamage, takeDamageFromWeaponGA.damage, Color.white);
                takeDamageFromWeaponGA.PostReactions.Add(spawnDamageUIPopupGA);
            }
        }

        yield return null;
    }
}
