using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DetailedUI : MonoBehaviour
{
    private Enemy enemy;
    private RectTransform statusEffectParent;

    private Slider healthBar;
    private TMP_Text healthValue;
    private Slider guardBar;
    private TMP_Text guardValue;
    private TMP_Text blockValue;


    public void Init()
    {
        healthBar = GetComponentsInChildren<Slider>()[0];
        guardBar = GetComponentsInChildren<Slider>()[1];
        healthValue = GetComponentsInChildren<TMP_Text>()[0];
        guardValue = GetComponentsInChildren<TMP_Text>()[1];
        blockValue = GetComponentsInChildren<TMP_Text>()[2];
        statusEffectParent = GetComponentsInChildren<RectTransform>()[5];
    }


    public void ChangeTarget(Enemy enemy)
    {
        this.enemy = enemy;
        DisplayStats();
    }

    public void DisplayStats()
    {
        if (enemy == null)
            return;

        healthBar.value = enemy.healthBar.value;
        healthValue.text = enemy.healthValue.text;

        guardBar.value = enemy.guardBar.value;
        guardValue.text = enemy.guardValue.text;

        blockValue.text = enemy.blockValue.text;
    }


}
