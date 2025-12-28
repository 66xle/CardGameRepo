using System.Collections;
using MyBox;
using UnityEngine;

public class GASystemCard : MonoBehaviour
{
    [MustBeAssigned][SerializeField] CardManager CardManager;

    private void OnEnable()
    {
        ActionSystem.AttachPerformer<GADrawCard>(DrawCardPerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<GADrawCard>();
    }

    private IEnumerator DrawCardPerformer(GADrawCard drawCardGA)
    {
        Avatar avatar = drawCardGA.Avatar;

        CardManager.DrawCards(drawCardGA.DrawAmount);

        yield return null;
    }
}
