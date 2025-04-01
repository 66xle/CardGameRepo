using DG.Tweening;
using UnityEngine;

public class PlayAnimGA : GameAction
{
    public Animator animator;
    public string animation;

    public PlayAnimGA(Animator animator, string animation)
    {
        this.animator = animator;
        this.animation = animation;
    }
}
