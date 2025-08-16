using System;
using System.Collections;
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
            }
            else
            {
                // Add game action to queue
                GainHealthGA gainHealthGA = new(avatarToHeal, heal);
                avatarToHeal.QueueGameActions.Add(gainHealthGA);

                if (avatarToHeal is Player)
                {
                    TogglePlayerUIGA togglePlayerUIGA = new(true);
                    gainHealthGA.PreReactions.Add(togglePlayerUIGA);
                }
                else
                {
                    ToggleEnemyUIGA toggleEnemyUIGA = new(true);
                    gainHealthGA.PreReactions.Add(toggleEnemyUIGA); // runs multiple times if there are multiple enemy targets
                }
            }

            ExecutableParameters.Targets[i] = avatarToHeal;
        }

        UpdateGameActionQueue();
    }
}
