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

    public void SelectGear(GearData data)
    {
        Title.text = data.GearName;
        StatValue.text = data.Value.ToString();
        FlavourDescription.text = data.Description;

        StatTitle.text = data is WeaponData ? StatTitle.text = "ATK" : StatTitle.text = "DEF";

        if (!data.Passive)
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
