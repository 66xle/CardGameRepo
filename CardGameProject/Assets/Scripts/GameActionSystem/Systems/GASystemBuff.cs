using System.Collections;
using MyBox;
using UnityEngine;

public class GASystemBuff : MonoBehaviour
{
    private void OnEnable()
    {
        ActionSystem.AttachPerformer<GAGainBlock>(GainBlockPerformer);
        ActionSystem.AttachPerformer<GAGainHealth>(GainHealthPerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<GAGainBlock>();
        ActionSystem.DetachPerformer<GAGainHealth>();
    }

    private IEnumerator GainBlockPerformer(GAGainBlock gainBlockGA)
    {
        Avatar avatarGainBlock = gainBlockGA.AvatarGainBlock;

        avatarGainBlock.AddBlock(gainBlockGA.BlockAmount);

        avatarGainBlock.UpdateStatsUI();

        yield return null;
    }

    private IEnumerator GainHealthPerformer(GAGainHealth gainHealthGA)
    {
        Avatar target = gainHealthGA.AvatarTarget;

        target.Heal(gainHealthGA.HealAmount);

        target.UpdateStatsUI();

        yield return null;
    }
}
