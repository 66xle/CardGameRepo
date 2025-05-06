using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatUIManager : MonoBehaviour
{
    [Foldout("Popup", true)]
    public GameObject damagePopupPrefab;
    public Transform worldSpaceCanvas;
    [Range(0, 1)] public float randomOffsetHorizontal = 0.5f;
    public float offsetVertical = 1f;

    [Separator("Animate")]
    public float moveDuration;
    public float fadeDuration;
    public float moveVertical;

    [Foldout("Button", true)]
    public Button endTurnButton;
    public Button switchButton;

    [Foldout("UI", true)]
    public GameObject combatUI;
    public GameObject switchWeaponUI;
    public GameObject HideUI;
    public GameObject DetailedUI;
    public GameObject PlayerUI;


    public void ToggleHideUI(bool toggle)
    {
        HideUI.SetActive(toggle);
        DetailedUI.SetActive(toggle);
        PlayerUI.SetActive(toggle);
    }
}
