using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public abstract class GuardCommand : Command
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

        for (int i = 0; i < ExecutableParameters.Targets.Count; i++)
        {
            Avatar avatarToTakeDamage = ExecutableParameters.Targets[i];

            if (avatarToTakeDamage.IsGameActionInQueue<GATakeGuardDamage>())
            {
                // Update damage value
                GATakeGuardDamage takeGuardDamageGA = avatarToTakeDamage.GetGameActionFromQueue<GATakeGuardDamage>() as GATakeGuardDamage;
                takeGuardDamageGA.GuardDamage += (int)Value;
            }
            else
            {
                // Add game action to queue
                GATakeGuardDamage takeGuardDamageGA = new(avatarToTakeDamage, Value, CardTarget);
                AddGameActionToQueue(takeGuardDamageGA, avatarToTakeDamage);
            }

            ExecutableParameters.Targets[i] = avatarToTakeDamage;
        }

        UpdateGameActionQueue();
    }
}
