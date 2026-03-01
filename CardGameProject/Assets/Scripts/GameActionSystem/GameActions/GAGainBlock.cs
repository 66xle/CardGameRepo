using UnityEngine;

public class GAGainBlock : GameAction
{
    public Avatar AvatarGainBlock;
    public float BlockAmount;


    public GAGainBlock(Avatar avatarGainBlock, float blockAmount)
    {
        AvatarGainBlock = avatarGainBlock;
        BlockAmount = blockAmount;
    }
}
