using System.Collections;
using MyBox;
using UnityEngine;

public class BuffSystem : MonoBehaviour
{
    private void OnEnable()
    {
        ActionSystem.AttachPerformer<GainBlockGA>(GainBlockPerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<GainBlockGA>();
    }

    private IEnumerator GainBlockPerformer(GainBlockGA gainBlockGA)
    {
        Avatar avatarGainBlock = gainBlockGA.AvatarGainBlock;

        avatarGainBlock.AddBlock(gainBlockGA.BlockAmount);

        avatarGainBlock.UpdateStatsUI();

        yield return null;
    }
}
