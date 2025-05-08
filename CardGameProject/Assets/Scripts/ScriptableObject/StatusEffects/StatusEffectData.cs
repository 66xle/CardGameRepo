using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SerializeReferenceEditor;

[CreateAssetMenu(fileName = "StatusEffectData", menuName = "StatusEffectData")]
public class StatusEffectData : ScriptableObject
{
    [SerializeReference][SR] public StatusEffect StatusEffect;
}
