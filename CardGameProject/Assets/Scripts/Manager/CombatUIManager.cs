using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatUIManager : MonoBehaviour
{
    [Foldout("Popup", true)]
    [MustBeAssigned] public GameObject DamagePopupPrefab;
    [MustBeAssigned] public Transform WorldSpaceCanvas;
    [Range(0, 1)] public float RandomOffsetHorizontal = 0.5f;
    public float OffsetVertical = 1f;
    public float baseScale = 1f;

    [Separator("Animate")]
    public float MoveDuration;
    public float FadeDuration;
    public float MoveVertical;

    [Foldout("Button", true)]
    [MustBeAssigned] public Button EndTurnButton;
    [MustBeAssigned] public Button SwitchButton;

    [Foldout("UI", true)]
    [MustBeAssigned] public GameObject CombatUI;
    [MustBeAssigned] public GameObject SwitchWeaponUI;
    [MustBeAssigned] public GameObject HideUI;
    [MustBeAssigned] public GameObject DetailedUI;
    [MustBeAssigned] public GameObject PlayerUI;


    public void ToggleHideUI(bool toggle)
    {
        HideUI.SetActive(toggle);
        DetailedUI.SetActive(toggle);
        PlayerUI.SetActive(toggle);
    }
}
