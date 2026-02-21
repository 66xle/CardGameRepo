using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GearSelect : MonoBehaviour
{
    [MustBeAssigned] [SerializeField] TMP_Text GearNameText;
    [MustBeAssigned] [SerializeField] RawImage GearIcon;
    [MustBeAssigned] [SerializeField] GameObject Highlight;
    private GearData GearData;

    public void Init(GearData data)
    {
        GearNameText.text = data.GearName;
        GearIcon.texture = data.IconTexture;
        GearData = data;
    }

    public void ToggleHighlight(bool toggle)
    {
        Highlight.SetActive(toggle);
    }

    public GearData GetGearData()
    {
        return GearData;
    }
}
