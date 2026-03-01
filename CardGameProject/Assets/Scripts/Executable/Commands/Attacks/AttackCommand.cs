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
        Avatar avatarPlayingCard = EXEParameters.AvatarPlayingCard;
        Avatar avatarOpponent = EXEParameters.AvatarOpponent;

        Animator avatarPlayingCardController = avatarPlayingCard.Animator;
        Animator opponentController = avatarOpponent.Animator;

        float damage = CalculateDamage.GetDamage(avatarPlayingCard.Attack, avatarPlayingCard.CurrentWeaponData.WeaponAttack, avatarOpponent, avatarPlayingCard, Value);

        for (int i = 0; i < EXEParameters.Targets.Count; i++)
        {
            Avatar avatarToTakeDamage = EXEParameters.Targets[i];

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
                GATakeDamageFromWeapon takeDamageFromWeaponGA = new(avatarToTakeDamage, damage, avatarPlayingCard.CurrentWeaponData.DamageType, EXEParameters.CardTarget);
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

            EXEParameters.Targets[i] = avatarToTakeDamage;
        }

        UpdateGameActionQueue();
    }
}
