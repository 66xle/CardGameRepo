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
        Avatar avatarPlayingCard = EXEParameters.AvatarPlayingCard;

        int heal = CalculateDamage.GetHealAmount(avatarPlayingCard.MaxHealth, Value);
        Debug.Log(heal);

        for (int i = 0; i < EXEParameters.Targets.Count; i++)
        {
            Avatar avatarToHeal = EXEParameters.Targets[i];

            if (avatarToHeal.IsGameActionInQueue<GAGainHealth>())
            {
                // Update damage value
                GAGainHealth gainHealthGA = avatarToHeal.GetGameActionFromQueue<GAGainHealth>() as GAGainHealth;
                gainHealthGA.HealAmount += heal;

                GASpawnDamageUIPopup spawnDamageUIPopupGA = gainHealthGA.PostReactions.First(gameAction => gameAction is GASpawnDamageUIPopup) as GASpawnDamageUIPopup;
                spawnDamageUIPopupGA.Text = gainHealthGA.HealAmount.ToString();
            }
            else
            {
                // Add game action to queue
                GAGainHealth gainHealthGA = new(avatarToHeal, heal);
                AddGameActionToQueue(gainHealthGA, avatarToHeal);

                GASpawnDamageUIPopup spawnDamageUIPopupGA = new(avatarToHeal, gainHealthGA.HealAmount.ToString(), Color.red);
                AddGameActionToQueue(spawnDamageUIPopupGA, avatarToHeal);
            }

            EXEParameters.Targets[i] = avatarToHeal;
        }

        UpdateGameActionQueue();
    }
}
