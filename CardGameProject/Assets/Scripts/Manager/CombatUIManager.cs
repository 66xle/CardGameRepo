using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatUIManager : MonoBehaviour
{
    [Separator("Popup")]
    public GameObject damagePopupPrefab;
    public Transform worldSpaceCanvas;
    [Range(0, 1)] public float randomHorizontalOffset = 0.5f;
    public float verticalOffset = 1f;

    [Separator("Button")]
    public Button endTurnButton;
    public Button switchButton;

    [Separator("UI")]
    public GameObject combatUI;
    public GameObject switchWeaponUI;
}
