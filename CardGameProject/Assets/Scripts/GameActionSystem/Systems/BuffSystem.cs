using System.Collections;
using MyBox;
using UnityEngine;

public class BuffSystem : MonoBehaviour
{
    private void OnEnable()
    {
        ActionSystem.AttachPerformer<GainBlockGA>(GainBlockPerformer);
        ActionSystem.AttachPerformer<GainHealthGA>(GainHealthPerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<GainBlockGA>();
        ActionSystem.DetachPerformer<GainHealthGA>();
    }

    private IEnumerator GainBlockPerformer(GainBlockGA gainBlockGA)
    {
        Avatar avatarGainBlock = gainBlockGA.AvatarGainBlock;

        avatarGainBlock.AddBlock(gainBlockGA.BlockAmount);

        avatarGainBlock.UpdateStatsUI();

        yield return null;
    }

    private IEnumerator GainHealthPerformer(GainHealthGA gainHealthGA)
    {
        Avatar target = gainHealthGA.AvatarTarget;

        target.Heal(gainHealthGA.HealAmount);

        target.UpdateStatsUI();

        yield return null;
    }
}
