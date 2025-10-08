using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Playables;

public class TriggerAnimGA : GameAction
{
    public Avatar AvatarPlayingCard;
    public string AnimationName;
    public PlayableAsset AnimTimeline;
    public AudioResource AudioResource;
    public bool EnableAttackCamera;

    public TriggerAnimGA(Avatar avatarPlayingCard, string animationName, PlayableAsset animTimeline, AudioResource audioResource)
    {
        AvatarPlayingCard = avatarPlayingCard;
        AnimationName = animationName;
        AnimTimeline = animTimeline;
        AudioResource = audioResource;
    }
}
