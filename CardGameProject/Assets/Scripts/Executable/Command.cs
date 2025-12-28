using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public abstract class Command : Executable
{
    public abstract override IEnumerator Execute(Action<bool> IsConditionTrue);

    public abstract void ExecuteCommand();

    protected void UpdateGameActionQueue()
    {
        // Update avatar queue game actions
        foreach (Avatar target in EXEParameters.Targets)
        {
            if (EXEParameters.Queue.Exists(avatar => avatar.Guid == target.Guid))
            {
                Avatar avatar = EXEParameters.Queue.First(avatar => avatar.Guid == target.Guid);
                avatar.QueueGameActions = target.QueueGameActions;
            }
            else
            {
                EXEParameters.Queue.Add(target);
            }
        }
    }

    protected void AddGameActionToQueue(GameAction gameAction, Avatar avatar, bool hideUI = true)
    {
        avatar.QueueGameActions.Add(gameAction);

        if (!hideUI) return;

        if (avatar is Player)
        {
            GATogglePlayerUI togglePlayerUIGA = new(true);
            gameAction.PreReactions.Add(togglePlayerUIGA);
        }
        else
        {
            GAToggleEnemyUI toggleEnemyUIGA = new(true);
            gameAction.PreReactions.Add(toggleEnemyUIGA); // runs multiple times if there are multiple enemy targets
        }
    }

}
