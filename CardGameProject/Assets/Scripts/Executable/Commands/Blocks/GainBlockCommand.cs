using System;
using System.Collections;
using UnityEngine;

public abstract class GainBlockCommand : Command
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

        for (int i = 0; i < ExecutableParameters.Targets.Count; i++)
        {
            Avatar avatarGainBlock = ExecutableParameters.Targets[i];

            if (avatarGainBlock.IsGameActionInQueue<GainBlockGA>())
            {
                // Update damage value
                GainBlockGA gainBlockGA = avatarGainBlock.GetGameActionFromQueue<GainBlockGA>() as GainBlockGA;
                gainBlockGA.BlockAmount += (int)Value;
            }
            else
            {
                // Add game action to queue
                GainBlockGA gainBlockGA = new(avatarGainBlock, Value);
                avatarGainBlock.QueueGameActions.Add(gainBlockGA);

                if (avatarGainBlock is Player)
                {
                    TogglePlayerUIGA togglePlayerUIGA = new(true);
                    gainBlockGA.PreReactions.Add(togglePlayerUIGA);
                }
                else
                {
                    ToggleEnemyUIGA toggleEnemyUIGA = new(true);
                    gainBlockGA.PreReactions.Add(toggleEnemyUIGA); // runs multiple times if there are multiple enemy targets
                }
            }

            ExecutableParameters.Targets[i] = avatarGainBlock;
        }

        UpdateGameActionQueue();
    }
}
