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

        float damage = CalculateDamage.GetDamage(avatarPlayingCard.Attack, avatarPlayingCard.CurrentWeaponData.WeaponAttack, avatarOpponent, avatarPlayingCard, Value);

        for (int i = 0; i < ExecutableParameters.Targets.Count; i++)
        {
            Avatar avatarToTakeDamage = ExecutableParameters.Targets[i];

            if (avatarToTakeDamage.IsGameActionInQueue<GATakeDamageFromWeapon>())
            {
                // Update damage value
                GATakeDamageFromWeapon takeDamageFromWeaponGA = avatarToTakeDamage.GetGameActionFromQueue<GATakeDamageFromWeapon>() as GATakeDamageFromWeapon;
                takeDamageFromWeaponGA.Damage += damage;

                GASpawnDamageUIPopup spawnDamageUIPopupGA = takeDamageFromWeaponGA.PostReactions.First(gameAction => gameAction is GASpawnDamageUIPopup) as GASpawnDamageUIPopup;
                spawnDamageUIPopupGA.Text = takeDamageFromWeaponGA.Damage.ToString();
            }
            else
            {
                // Add game action to queue
                GATakeDamageFromWeapon takeDamageFromWeaponGA = new(avatarToTakeDamage, damage, avatarPlayingCard.CurrentWeaponData.DamageType, ExecutableParameters.CardTarget);
                AddGameActionToQueue(takeDamageFromWeaponGA, avatarToTakeDamage);

                GASpawnDamageUIPopup spawnDamageUIPopupGA = new(takeDamageFromWeaponGA.AvatarToTakeDamage, takeDamageFromWeaponGA.Damage.ToString(), Color.white);
                AddGameActionToQueue(spawnDamageUIPopupGA, avatarToTakeDamage);

                if (avatarOpponent.IsInCounterState)
                {
                    Debug.Log("Is in counter state");
                    GACounter counterGA = new(avatarOpponent, avatarPlayingCard);
                    avatarOpponent.QueueGameActions.Add(counterGA);

                    AddGameActionToQueue(counterGA, avatarOpponent);
                }
            }

            ExecutableParameters.Targets[i] = avatarToTakeDamage;
        }

        UpdateGameActionQueue();
    }
}
