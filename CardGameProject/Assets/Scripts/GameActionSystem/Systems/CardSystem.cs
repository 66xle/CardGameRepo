using System.Collections;
using MyBox;
using UnityEngine;

public class CardSystem : MonoBehaviour
{
    [MustBeAssigned][SerializeField] CardManager CardManager;

    private void OnEnable()
    {
        ActionSystem.AttachPerformer<DrawCardGA>(DrawCardPerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<DrawCardGA>();
    }

    private IEnumerator DrawCardPerformer(DrawCardGA drawCardGA)
    {
        Avatar avatar = drawCardGA.Avatar;

        CardManager.DrawCards(drawCardGA.DrawAmount);

        yield return null;
    }
}
