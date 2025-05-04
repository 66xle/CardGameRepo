using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class AttackCommand : Command
{
    public override float Value { get; }

    public override IEnumerator Execute(Action<bool> IsConditionTrue)
    {
        ExecuteCommand();

        yield return null;
    }

    public override void ExecuteCommand()
    {
        CombatStateMachine ctx = ExecutableParameters.ctx;
        Avatar avatarPlayingCard = ExecutableParameters.avatarPlayingCard;
        Avatar avatarOpponent = ExecutableParameters.avatarOpponent;

        Animator avatarPlayingCardController = avatarPlayingCard.GetComponent<Animator>();
        Animator opponentController = avatarOpponent.GetComponent<Animator>();

        if (avatarOpponent.isInCounterState)
        {
            CounterGA counterGA = new(avatarOpponent, avatarPlayingCardController, opponentController);
            avatarOpponent.queueGameActions.Add(counterGA);
        }
        else
        {
            for (int i = 0; i < ExecutableParameters.Targets.Count; i++)
            {
                Avatar avatarToTakeDamage = ExecutableParameters.Targets[i];

                if (avatarToTakeDamage.IsGameActionInQueue<TakeDamageFromWeaponGA>())
                {
                    // Update damage value
                    TakeDamageFromWeaponGA takeDamageFromWeaponGA = avatarToTakeDamage.GetGameActionFromQueue<TakeDamageFromWeaponGA>() as TakeDamageFromWeaponGA;
                    takeDamageFromWeaponGA.damage += Value;

                    SpawnDamageUIPopupGA spawnDamageUIPopupGA = takeDamageFromWeaponGA.PostReactions.First(gameAction => gameAction is SpawnDamageUIPopupGA) as SpawnDamageUIPopupGA;
                    spawnDamageUIPopupGA.damage = takeDamageFromWeaponGA.damage;
                }
                else
                {
                    // Add game action to queue
                    TakeDamageFromWeaponGA takeDamageFromWeaponGA = new(avatarToTakeDamage, ctx, Value, ExecutableParameters.weapon.type, ExecutableParameters.CardTarget);
                    avatarToTakeDamage.queueGameActions.Add(takeDamageFromWeaponGA);

                    SpawnDamageUIPopupGA spawnDamageUIPopupGA = new(takeDamageFromWeaponGA.avatarToTakeDamage, takeDamageFromWeaponGA.damage, Color.white);
                    takeDamageFromWeaponGA.PostReactions.Add(spawnDamageUIPopupGA);

                    avatarToTakeDamage.isTakeDamage = true;
                }

                ExecutableParameters.Targets[i] = avatarToTakeDamage;
            }

            // Update avatar queue game actions
            foreach (Avatar target in ExecutableParameters.Targets)
            {
                if (ExecutableParameters.Queue.Exists(avatar => avatar.guid == target.guid))
                {
                    Avatar avatar = ExecutableParameters.Queue.First(avatar => avatar.guid == target.guid);
                    avatar.queueGameActions = target.queueGameActions;
                }
                else
                {
                    ExecutableParameters.Queue.Add(target);
                }
            }
        }
    }
}
