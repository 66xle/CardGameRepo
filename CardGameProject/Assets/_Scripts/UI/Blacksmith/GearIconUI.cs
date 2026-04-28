using System;
using UnityEngine;
using UnityEngine.UI;

public class GearIconUI : MonoBehaviour
{
    private GearRuntime _gearRuntime;

    public void SetData(GearRuntime gearRuntime, Action<GearRuntime> onClickSelectIcon)
    {
        _gearRuntime = gearRuntime;
        RawImage image = GetComponent<RawImage>();
        image.texture = gearRuntime.GearData.IconTexture;
        Button button = GetComponent<Button>();
        button.onClick.AddListener(() => onClickSelectIcon?.Invoke(gearRuntime));
    }
}
