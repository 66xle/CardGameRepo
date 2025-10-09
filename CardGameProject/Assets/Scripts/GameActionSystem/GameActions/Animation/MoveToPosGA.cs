using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

public class MoveToPosGA : GameAction
{
    public Avatar AvatarPlayingCard;
    public Avatar AvatarOpponent;
    public bool MoveToCenter;
    public float DistanceOffset;
    public GameObject FollowTimeline;
    public float MoveTime;

    public MoveToPosGA(Avatar avatarToMove, Avatar avatarOpponent, bool moveToCenter, float distanceOffset, GameObject followTimeline, float moveTime)
    {
        AvatarPlayingCard = avatarToMove;
        AvatarOpponent = avatarOpponent;
        MoveToCenter = moveToCenter;
        DistanceOffset = distanceOffset;
        FollowTimeline = followTimeline;
        MoveTime = moveTime;
    }
}
