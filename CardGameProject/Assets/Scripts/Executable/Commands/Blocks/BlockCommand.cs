using System;
using System.Collections;
using UnityEngine;

public abstract class BlockCommand : Command
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

        float block = CalculateDamage.GetBlock(avatarPlayingCard.Defence, Value, avatarPlayingCard.BlockScale);

        for (int i = 0; i < ExecutableParameters.Targets.Count; i++)
        {
            Avatar avatarGainBlock = ExecutableParameters.Targets[i];

            if (avatarGainBlock.IsGameActionInQueue<GAGainBlock>())
            {
                // Update damage value
                GAGainBlock gainBlockGA = avatarGainBlock.GetGameActionFromQueue<GAGainBlock>() as GAGainBlock;
                gainBlockGA.BlockAmount += (int)block;
            }
            else
            {
                // Add game action to queue
                GAGainBlock gainBlockGA = new(avatarGainBlock, block);
                AddGameActionToQueue(gainBlockGA, avatarGainBlock);
            }

            ExecutableParameters.Targets[i] = avatarGainBlock;
        }

        UpdateGameActionQueue();
    }
}
