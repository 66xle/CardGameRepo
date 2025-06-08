using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DetailedUI : MonoBehaviour
{
    private Enemy _enemy;
    private CombatStateMachine _ctx;

    private Image _healthBar;
    private TMP_Text healthValue;
    private Image _guardBar;
    private TMP_Text guardValue;
    private TMP_Text blockValue;

    public GameObject ActiveParent;
    public GameObject DeactiveParent;
    public GameObject UiPrefab;
    public float EffectUIOffset = 50f;


    public void Init(CombatStateMachine ctx)
    {
        _healthBar = GetComponentsInChildren<Image>()[1];
        _guardBar = GetComponentsInChildren<Image>()[2];
        //healthValue = GetComponentsInChildren<TMP_Text>()[0];
        //guardValue = GetComponentsInChildren<TMP_Text>()[1];
        //blockValue = GetComponentsInChildren<TMP_Text>()[2];

        _ctx = ctx;
    }


    public void ChangeTarget(Enemy enemy)
    {
        _enemy = enemy;
        DisplayStats();
        ClearStatusUI();
        UpdateStatusEffectsUI();
    }

    public void DisplayStats()
    {
        if (_enemy == null)
            return;

        _healthBar.fillAmount = _enemy.HealthBar.fillAmount;
        //healthValue.text = enemy.healthValue.text;

        _guardBar.fillAmount = _enemy.GuardBar.fillAmount;
        //guardValue.text = enemy.guardValue.text;

        //blockValue.text = enemy.blockValue.text;
    }

    public void UpdateStatusEffectsUI()
    {
        if (_enemy == null)
            return;

        if (_ctx._selectedEnemyToAttack != _enemy)
            return;

        for (int i = 0; i < _enemy.ListOfEffects.Count; i++)
        {
            StatusEffect data = _enemy.ListOfEffects[i];

            // Check activeParent childs
            GameObject activeObj = GetEffectObject(ActiveParent, data.EffectName);
            if (activeObj != null)
            {
                // Update Effect
                UpdateStatusUI(i, activeObj, data);
                continue;
            }

            // Check deactiveParent childs
            GameObject deactiveObj = GetEffectObject(DeactiveParent, data.EffectName);
            if (deactiveObj != null)
            {
                // Set to active and update effect
                deactiveObj.transform.parent = ActiveParent.transform;
                deactiveObj.SetActive(true);

                UpdateStatusUI(i, deactiveObj, data);
                continue;
            }

            // Instanitate object in active
            GameObject newEffectObj = Instantiate(UiPrefab, ActiveParent.transform);
            newEffectObj.tag = data.EffectName;
            UpdateStatusUI(i, newEffectObj, data);
        }
    }
    
    public void ClearStatusUI()
    {
        int parentCount = ActiveParent.transform.childCount;

        for (int i = 0; i < parentCount; i++)
        {
            Transform effectTransform = ActiveParent.transform.GetChild(0);
            effectTransform.parent = DeactiveParent.transform;
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

    void UpdateStatusUI(int index, GameObject effectObj, StatusEffect effect)
    {
        effectObj.GetComponent<RectTransform>().localPosition = new Vector3(ActiveParent.transform.localPosition.x + (EffectUIOffset * index), ActiveParent.transform.localPosition.y, ActiveParent.transform.localPosition.z);
        
        if (effect.GetStacks() > 0)
            effectObj.GetComponentInChildren<TMP_Text>().text = effect.GetStacks().ToString();

    }


}
