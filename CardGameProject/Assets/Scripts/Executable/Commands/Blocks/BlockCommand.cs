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
        Animator avatarPlayingCardController = avatarPlayingCard.GetComponent<Animator>();

        float block = CalculateDamage.GetBlock(avatarPlayingCard.Defence, Value, avatarPlayingCard.BlockScale);

        for (int i = 0; i < ExecutableParameters.Targets.Count; i++)
        {
            Avatar avatarGainBlock = ExecutableParameters.Targets[i];

            if (avatarGainBlock.IsGameActionInQueue<GainBlockGA>())
            {
                // Update damage value
                GainBlockGA gainBlockGA = avatarGainBlock.GetGameActionFromQueue<GainBlockGA>() as GainBlockGA;
                gainBlockGA.BlockAmount += (int)block;
            }
            else
            {
                // Add game action to queue
                GainBlockGA gainBlockGA = new(avatarGainBlock, block);
                AddGameActionToQueue(gainBlockGA, avatarGainBlock);
            }

            ExecutableParameters.Targets[i] = avatarGainBlock;
        }

        UpdateGameActionQueue();
    }
}
