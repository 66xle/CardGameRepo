using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DetailedUI : MonoBehaviour
{
    private Enemy enemy;
    private CombatStateMachine state;

    

    private Image healthBar;
    private TMP_Text healthValue;
    private Image guardBar;
    private TMP_Text guardValue;
    private TMP_Text blockValue;

    public GameObject activeParent;
    public GameObject deactiveParent;
    public float effectUIOffset = 50f;


    public void Init(CombatStateMachine state)
    {
        healthBar = GetComponentsInChildren<Image>()[1];
        guardBar = GetComponentsInChildren<Image>()[2];
        //healthValue = GetComponentsInChildren<TMP_Text>()[0];
        //guardValue = GetComponentsInChildren<TMP_Text>()[1];
        //blockValue = GetComponentsInChildren<TMP_Text>()[2];

        this.state = state;
    }


    public void ChangeTarget(Enemy enemy)
    {
        this.enemy = enemy;
        DisplayStats();
        ResetStatusUI();
        UpdateEffectUI();
    }

    public void DisplayStats()
    {
        if (enemy == null)
            return;

        healthBar.fillAmount = enemy.healthBar.fillAmount;
        //healthValue.text = enemy.healthValue.text;

        guardBar.fillAmount = enemy.guardBar.fillAmount;
        //guardValue.text = enemy.guardValue.text;

        //blockValue.text = enemy.blockValue.text;
    }

    public void UpdateEffectUI()
    {
        if (enemy == null)
            return;

        if (state.selectedEnemyToAttack != enemy)
            return;

        for (int i = 0; i < enemy.listOfEffects.Count; i++)
        {
            StatusEffectData data = enemy.listOfEffects[i];

            GameObject activeObj = GetEffectObject(activeParent, data.effectName);
            if (activeObj != null)
            {
                // Update Effect
                UpdateStatusUI(i, activeObj, data);
                continue;
            }

            GameObject deactiveObj = GetEffectObject(deactiveParent, data.effectName);
            if (deactiveObj != null)
            {
                // Set to active and update effect
                deactiveObj.transform.parent = activeParent.transform;
                deactiveObj.SetActive(true);

                UpdateStatusUI(i, deactiveObj, data);
                continue;
            }

            // Instanitate object in active
            GameObject newEffectObj = Instantiate(data.uiPrefab, activeParent.transform);
            newEffectObj.tag = data.effectName;
            UpdateStatusUI(i, newEffectObj, data);
        }
    }
    
    public void ResetStatusUI()
    {
        int parentCount = activeParent.transform.childCount;

        for (int i = 0; i < parentCount; i++)
        {
            Transform effectTransform = activeParent.transform.GetChild(0);
            effectTransform.parent = deactiveParent.transform;
            effectTransform.gameObject.SetActive(false);
        }
    }

    GameObject GetEffectObject(GameObject parentToCheck, string effectName)
    {
        for (int i = 0; i < parentToCheck.transform.childCount; i++)
        {
            if (effectName == parentToCheck.transform.GetChild(i).tag)
            {
                return parentToCheck.transform.GetChild(i).gameObject;
            }
        }

        return null;
    }

    void UpdateStatusUI(int index, GameObject effectObj, StatusEffectData data)
    {
        effectObj.GetComponent<RectTransform>().position = new Vector3(activeParent.transform.position.x + (effectUIOffset * index), activeParent.transform.position.y, activeParent.transform.position.z);
        effectObj.GetComponentInChildren<TMP_Text>().text = data.turnRemaining.ToString();
    }


}
