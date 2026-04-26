using SerializeReferenceEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "StatusEffectData", menuName = "StatusEffectData")]
public class StatusEffectData : ScriptableObject
{
    [SerializeReference][SR] public StatusEffect StatusEffect;
}
