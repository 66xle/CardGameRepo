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
        foreach (Avatar target in ExecutableParameters.Targets)
        {
            if (ExecutableParameters.Queue.Exists(avatar => avatar.Guid == target.Guid))
            {
                Avatar avatar = ExecutableParameters.Queue.First(avatar => avatar.Guid == target.Guid);
                avatar.QueueGameActions = target.QueueGameActions;
            }
            else
            {
                ExecutableParameters.Queue.Add(target);
            }
        }
    }
}
