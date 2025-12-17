using MyBox;
using TMPro;
using UnityEngine;

public class GearSelectionUI : MonoBehaviour
{
    [MustBeAssigned] [SerializeField] TMP_Text Title;
    [MustBeAssigned] [SerializeField] TMP_Text Stats;
    [MustBeAssigned] [SerializeField] TMP_Text Passive;
    [MustBeAssigned] [SerializeField] TMP_Text FlavourDescription;

    public void SelectGear(GearData data)
    {
        Title.text = data.GearName;
        Stats.text = data.Value.ToString();
    }
}
