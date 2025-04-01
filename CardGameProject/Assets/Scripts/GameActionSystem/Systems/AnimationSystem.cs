using DG.Tweening;
using System.Collections;
using UnityEngine;

public class AnimationSystem : MonoBehaviour
{
    public bool IsAnimationFinished;

    private void OnEnable()
    {
        ActionSystem.AttachPerformer<PlayAnimGA>(PlayAnimPerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<PlayAnimGA>();
    }

    private IEnumerator PlayAnimPerformer(PlayAnimGA playAnim)
    {
        

        


        yield return null;
    }

    
}
