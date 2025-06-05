using MyBox;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

[CreateAssetMenu(fileName = "New Popup", menuName = "Card Popup")]
public class PopupText : ScriptableObject
{
    public StatusEffectData StatusEffectData;

    [Separator]


    public string Title;
    [TextArea] public string Description;
    [ReadOnly][TextArea] public string DisplayDescription;
    private void OnValidate()
    {
        DisplayDescription = Description;
        DisplayDescription = DisplayDescription.Replace($"#{Title}", $"{StatusEffectData.StatusEffect.GetDataPopup()}");
    }
}
