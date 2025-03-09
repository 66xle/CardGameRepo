using UnityEngine;


[CreateAssetMenu(fileName = "Poison", menuName = "StatusEffect/Poison")]

public class StatusPoison : StatusEffect
{
    [Range(0, 1)] public float reduceHealthPercentage;

    public StatusPoison()
    {
        effectName = "Poison";
        effect = Effect.Poison;
    }
}
