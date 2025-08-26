using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public abstract class HealCommand : Command
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
        Animator avatarPlayingCardController = avatarPlayingCard.GetComponent<Animator>();

        int heal = CalculateDamage.GetHealAmount(avatarPlayingCard.MaxHealth, Value);
        Debug.Log(heal);

        for (int i = 0; i < ExecutableParameters.Targets.Count; i++)
        {
            Avatar avatarToHeal = ExecutableParameters.Targets[i];

            if (avatarToHeal.IsGameActionInQueue<GainHealthGA>())
            {
                // Update damage value
                GainHealthGA gainHealthGA = avatarToHeal.GetGameActionFromQueue<GainHealthGA>() as GainHealthGA;
                gainHealthGA.HealAmount += heal;

                SpawnDamageUIPopupGA spawnDamageUIPopupGA = gainHealthGA.PostReactions.First(gameAction => gameAction is SpawnDamageUIPopupGA) as SpawnDamageUIPopupGA;
                spawnDamageUIPopupGA.Damage = gainHealthGA.HealAmount;
            }
            else
            {
                // Add game action to queue
                GainHealthGA gainHealthGA = new(avatarToHeal, heal);
                AddGameActionToQueue(gainHealthGA, avatarToHeal);

                SpawnDamageUIPopupGA spawnDamageUIPopupGA = new(avatarToHeal, gainHealthGA.HealAmount, Color.red);
                AddGameActionToQueue(spawnDamageUIPopupGA, avatarToHeal);
            }

            ExecutableParameters.Targets[i] = avatarToHeal;
        }

        UpdateGameActionQueue();
    }
}
