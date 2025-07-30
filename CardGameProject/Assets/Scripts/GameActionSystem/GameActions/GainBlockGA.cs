using UnityEngine;

public class GainBlockGA : GameAction
{
    public Avatar AvatarGainBlock;
    public float BlockAmount;


    public GainBlockGA(Avatar avatarGainBlock, float blockAmount)
    {
        AvatarGainBlock = avatarGainBlock;
        BlockAmount = blockAmount;
    }
}
