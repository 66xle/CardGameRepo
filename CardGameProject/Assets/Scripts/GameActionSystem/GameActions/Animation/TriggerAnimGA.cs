using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Playables;

public class TriggerAnimGA : GameAction
{
    public Avatar AvatarPlayingCard;
    public string AnimationName;
    public GameObject AnimTimeline;
    public AudioResource AudioResource;

    public TriggerAnimGA(Avatar avatarPlayingCard, string animationName, GameObject animTimeline, AudioResource audioResource)
    {
        AvatarPlayingCard = avatarPlayingCard;
        AnimationName = animationName;
        AnimTimeline = animTimeline;
        AudioResource = audioResource;
    }
}
