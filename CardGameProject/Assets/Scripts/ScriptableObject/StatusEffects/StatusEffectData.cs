using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectData
{
    public Effect effect;
    public string effectName;
    public int turnRemaining;
    public float reduceDamagePercentage;
    public float extraDamagePercentage;
    public int numberOfHitsToRecover;

    public bool removeEffectNextTurn;

    public StatusEffectData(Effect effect, string name, int turnRemaining, float reduceDmgPer = 0f, int numHitToRecover = 0, float extraDmgPer = 0f, bool nextTurn = false)
    {
        // Required
        this.effect = effect;
        effectName = name;
        this.turnRemaining = turnRemaining;

        // Optional
        reduceDamagePercentage = reduceDmgPer;
        numberOfHitsToRecover = numHitToRecover;
        extraDamagePercentage = extraDmgPer;
        removeEffectNextTurn = nextTurn;
    }

}
