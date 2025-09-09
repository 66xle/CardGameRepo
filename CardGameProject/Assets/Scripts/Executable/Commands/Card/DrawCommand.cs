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
        for (int i = 0; i < ExecutableParameters.Targets.Count; i++)
        {
            Avatar avatar = ExecutableParameters.Targets[i];

            if (avatar.IsGameActionInQueue<DrawCardGA>())
            {
                // Update damage value
                DrawCardGA drawCardGA = avatar.GetGameActionFromQueue<DrawCardGA>() as DrawCardGA;
                drawCardGA.DrawAmount += (int)Value;
            }
            else
            {
                // Add game action to queue
                DrawCardGA drawCardGA = new(avatar, (int)Value);
                AddGameActionToQueue(drawCardGA, avatar, false);
            }

            ExecutableParameters.Targets[i] = avatar;
        }

        UpdateGameActionQueue();
    }
}
