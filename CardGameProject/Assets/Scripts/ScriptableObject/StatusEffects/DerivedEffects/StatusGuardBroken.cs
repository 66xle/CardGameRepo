using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GuardBroken", menuName = "StatusEffect/GuardBroken")]
public class StatusGuardBroken : StatusEffect
{
    public int numberOfHitsToRecover;
    [Range(0, 1)] public float extraDamagePercentage;

    public StatusGuardBroken()
    {
        effectName = "Guard Broken";
        effect = Effect.GuardBroken;
    }
}
