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
        Avatar avatarPlayingCard = ExecutableParameters.AvatarPlayingCard;
        Avatar avatarOpponent = ExecutableParameters.AvatarOpponent;

        Animator avatarPlayingCardController = avatarPlayingCard.GetComponent<Animator>();
        Animator opponentController = avatarOpponent.GetComponent<Animator>();

        if (avatarOpponent.IsInCounterState)
        {
            CounterGA counterGA = new(avatarOpponent, avatarPlayingCardController, opponentController);
            avatarOpponent.QueueGameActions.Add(counterGA);
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
                    takeDamageFromWeaponGA.Damage += Value;

                    SpawnDamageUIPopupGA spawnDamageUIPopupGA = takeDamageFromWeaponGA.PostReactions.First(gameAction => gameAction is SpawnDamageUIPopupGA) as SpawnDamageUIPopupGA;
                    spawnDamageUIPopupGA.Damage = takeDamageFromWeaponGA.Damage;
                }
                else
                {
                    // Add game action to queue
                    TakeDamageFromWeaponGA takeDamageFromWeaponGA = new(avatarToTakeDamage, Value, ExecutableParameters.WeaponData.Type, ExecutableParameters.CardTarget);
                    avatarToTakeDamage.QueueGameActions.Add(takeDamageFromWeaponGA);

                    SpawnDamageUIPopupGA spawnDamageUIPopupGA = new(takeDamageFromWeaponGA.AvatarToTakeDamage, takeDamageFromWeaponGA.Damage, Color.white);
                    takeDamageFromWeaponGA.PostReactions.Add(spawnDamageUIPopupGA);

                    if (avatarToTakeDamage is Player)
                    {
                        TogglePlayerUIGA togglePlayerUIGA = new(true);
                        takeDamageFromWeaponGA.PreReactions.Add(togglePlayerUIGA);
                    }
                    else
                    {
                        ToggleEnemyUIGA toggleEnemyUIGA = new(true);
                        takeDamageFromWeaponGA.PreReactions.Add(toggleEnemyUIGA); // runs multiple times if there are multiple enemy targets
                    }
                }

                ExecutableParameters.Targets[i] = avatarToTakeDamage;
            }

            UpdateGameActionQueue();
        }
    }
}
