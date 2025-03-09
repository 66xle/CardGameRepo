using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bleed", menuName = "StatusEffect/Bleed")]
public class StatusBleed : StatusEffect
{
    [Range(0, 1)] public float reduceHealthPercentage;
    public bool stackable;

    public StatusBleed()
    {
        effectName = "Bleed";
        effect = Effect.Bleed;
    }
}
