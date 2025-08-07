using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GearSelect : MonoBehaviour
{
    [MustBeAssigned] [SerializeField] TMP_Text GearNameText;
    [MustBeAssigned] [SerializeField] RawImage GearIcon;
    [MustBeAssigned] [SerializeField] GameObject Highlight;
    private WeaponData WeaponData;

    public void Init(WeaponData data)
    {
        GearNameText.text = data.GearName;
        GearIcon.texture = data.IconTexture;
        WeaponData = data;
    }

    public void ToggleHighlight(bool toggle)
    {
        Highlight.SetActive(toggle);
    }

    public WeaponData GetWeaponData()
    {
        return WeaponData;
    }

}
