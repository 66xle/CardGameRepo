using System;
using System.Collections;
using UnityEngine;

public abstract class DrawCommand : Command
{
    public override float Value { get; }

    public override IEnumerator Execute(Action<bool> IsConditionTrue)
    {
        ExecuteCommand();

        yield return null;
    }

    public override void ExecuteCommand()
    {
        for (int i = 0; i < EXEParameters.Targets.Count; i++)
        {
            Avatar avatar = EXEParameters.Targets[i];

            if (avatar.IsGameActionInQueue<GADrawCard>())
            {
                // Update damage value
                GADrawCard drawCardGA = avatar.GetGameActionFromQueue<GADrawCard>() as GADrawCard;
                drawCardGA.DrawAmount += (int)Value;
            }
            else
            {
                // Add game action to queue
                GADrawCard drawCardGA = new(avatar, (int)Value);
                AddGameActionToQueue(drawCardGA, avatar, false);
            }

            EXEParameters.Targets[i] = avatar;
        }

        UpdateGameActionQueue();
    }
}
