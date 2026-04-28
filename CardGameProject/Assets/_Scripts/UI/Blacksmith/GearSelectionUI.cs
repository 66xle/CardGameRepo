using MyBox;
using TMPro;
using UnityEngine;

public class GearSelectionUI : MonoBehaviour
{
    [MustBeAssigned][SerializeField] public TMP_Text Title;
    [MustBeAssigned][SerializeField] public TMP_Text StatTitle;
    [MustBeAssigned][SerializeField] public TMP_Text StatValue;
    [MustBeAssigned][SerializeField] public TMP_Text Passive;
    [MustBeAssigned][SerializeField] public TMP_Text FlavourDescription;

    public void SelectGear(GearRuntime gearRuntime)
    {
        Title.text = gearRuntime.GearData.GearName;
        StatValue.text = gearRuntime.GearData.Value.ToString();
        FlavourDescription.text = gearRuntime.GearData.Description;

        StatTitle.text = gearRuntime.GearData is WeaponData ? StatTitle.text = "ATK" : StatTitle.text = "DEF";

        if (!gearRuntime.GearData.Passive)
        {
            Passive.gameObject.SetActive(false);
        }
        else
        {
            // Set passive text here
            Passive.gameObject.SetActive(true);
        }
    }
}
