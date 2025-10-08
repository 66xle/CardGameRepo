using UnityEngine;
using UnityEngine.Playables;

public class TriggerAnimGA : GameAction
{
    public Avatar AvatarPlayingCard;
    public string AnimationName;
    public PlayableAsset AnimTimeline;
    public AudioType AudioType;
    public bool EnableAttackCamera;

    public TriggerAnimGA(Avatar avatarPlayingCard, string animationName, PlayableAsset animTimeline, AudioType audioType)
    {
        AvatarPlayingCard = avatarPlayingCard;
        AnimationName = animationName;
        AnimTimeline = animTimeline;
        AudioType = audioType;
    }
}
