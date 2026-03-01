using UnityEngine;
using UnityEngine.UI;
using System;

public class GearIconUI : MonoBehaviour
{
    GearData GearData;

    public void SetData(GearData data, Action<GearData> onClickSelectIcon)
    {
        GearData = data;
        RawImage image = GetComponent<RawImage>();
        image.texture = data.IconTexture;

        Button button = GetComponent<Button>();
        button.onClick.AddListener(() => onClickSelectIcon?.Invoke(GearData));
    }
}
