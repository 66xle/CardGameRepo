using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Android.Gradle.Manifest;
using UnityEngine;
using System;
using Action = System.Action;
using MyBox;
using UnityEngine.Rendering;



public class Avatar : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 100f;
    public int maxGuard = 10;
    public ArmourType armourType;
    public DamageType damageType;

    // Animation Events
    [HideInInspector] public bool doDamage;
    [HideInInspector] public bool isAttackFinished;
    [HideInInspector] public bool isTakeDamage = false;
    [ReadOnly] public bool isRunningReactiveEffect = false;

    [HideInInspector] public bool isInCounterState = false;
    [HideInInspector] public bool isInStatusActivation;
    [HideInInspector] public bool doStatusDmg;

    [HideInInspector] public event Action OnStatChanged;
    [HideInInspector] public Dictionary<ReactiveTrigger, List<ExecutableWrapper>> DictReactiveEffects = new();

    [HideInInspector] public List<StatusEffect> listOfEffects = new List<StatusEffect>();
    [HideInInspector] public List<GameAction> queueGameActions = new();

    [HideInInspector] public string guid;

    [MyBox.ReadOnly] public float _currentHealth;
    protected float _currentBlock;
    protected int _currentGuard;

    [HideInInspector] public Animator animator;

    protected float CurrentHealth { get { return _currentHealth; } set { _currentHealth = value; UpdateStatsUI(); } }
    protected float CurrentBlock { get { return _currentBlock; } set { _currentBlock = value; UpdateStatsUI(); } }
    protected int CurrentGuard { get { return _currentGuard; } set { _currentGuard = value; UpdateStatsUI(); } }


    private void Awake()
    {
        guid = Guid.NewGuid().ToString();
        animator = GetComponent<Animator>();
    }


    #region Avatar Methods

    #region Take Damage

    public void TakeDamage(float damage) 
    {
        damage = ApplyAdditionalDmgCheck(damage);

        _currentBlock -= damage;

        // Block is negative do damage / Block is positive dont do damage
        damage = _currentBlock < 0 ? Mathf.Abs(_currentBlock) : 0;

        if (_currentBlock < 0) _currentBlock = 0;

        _currentHealth -= damage;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, maxHealth);
    }

    public void TakeDamageByStatusEffect(float damage)
    {
        damage = ApplyAdditionalDmgCheck(damage);

        _currentHealth -= damage;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, maxHealth);
    }

    #endregion

    #region Guard

    public bool IsGuardReducible(DamageType damageType)
    {
        if (armourType == ArmourType.Light && damageType == DamageType.Slash ||
            armourType == ArmourType.Medium && damageType == DamageType.Pierce ||
            armourType == ArmourType.Heavy && damageType == DamageType.Blunt ||
            armourType == ArmourType.None)
        {
            return true;
        }

        return false;
    }

    public bool IsGuardBroken()
    {
        return _currentGuard == 0 ? true : false;
    }

    public void ReduceGuard()
    {
        _currentGuard--;
        _currentGuard = Mathf.Clamp(_currentGuard, 0, maxGuard);
    }

    public virtual void RecoverGuardBreak()
    {
        _currentGuard = maxGuard;
    }

    #endregion

    public bool IsAvatarDead()
    {
        if (_currentHealth <= 0f)
        {
            return true;
        }

        return false;
    }

    public void AddBlock(float block)
    {
        _currentBlock += block;
    }

    public void Heal(float healAmount)
    {
        _currentHealth += healAmount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, maxHealth);
    }

    #endregion

    #region Apply Status Effects

    public void ApplyStatusEffect(StatusEffect statusEffect)
    {
        if (listOfEffects.Any(status => status.effect == statusEffect.effect))
        {
            int index = listOfEffects.FindIndex(status => status.effect == statusEffect.effect);
            listOfEffects[index].OnApply(this);
        }
        else
        {
            listOfEffects.Add(statusEffect);
            statusEffect.OnApply(this);
        }
        
        UpdateStatsUI();
    }

    #endregion

    #region Status Effects Methods

    public bool hasStatusEffect(Effect effect)
    {
        foreach (StatusEffect data in listOfEffects)
        {
            if (data.effect == effect)
            {
                return true;
            }
        }

        return false;
    }

    public float ApplyAdditionalDmgCheck(float damage)
    {
        // Don't need to check
        if (listOfEffects.Count == 0) return damage;

        foreach (StatusEffect statusEffect in listOfEffects)
        {
            if (statusEffect.effect == Effect.GuardBroken)
            {
                StatusGuardBroken statusGuardBroken = statusEffect as StatusGuardBroken;
                return damage * statusGuardBroken.extraDamagePercentage + damage;
            }
        }

        return damage;
    }

    #endregion

    #region Game Action

    public bool IsGameActionInQueue<T>() where T : GameAction
    {
        return queueGameActions.Any(gameAction => gameAction is T);
    }

    public GameAction GetGameActionFromQueue<T>() where T : GameAction
    {
        return queueGameActions.First(gameAction => gameAction is T);
    }

    #endregion

    #region Reactive Effects

    public IEnumerator CheckReactiveEffects(ReactiveTrigger trigger)
    {
        if (!DictReactiveEffects.ContainsKey(trigger)) yield break;

        List<ExecutableWrapper> listWrapper = Extensions.CloneList(DictReactiveEffects[trigger]);

        for (int i = 0; i < listWrapper.Count; i++)
        {
            ExecutableWrapper wrapper = listWrapper[i];

            if (IsGuardBroken() && wrapper.Commands.Any(command => command is AttackCommand)) continue;

            
            if (wrapper.EffectTiming == EffectTiming.Immediate && trigger == wrapper.ReactiveTrigger)
            {
                // run commands
                isRunningReactiveEffect = true;

                yield return wrapper.ExecuteCommands();

                isRunningReactiveEffect = false;
            }
            else if (wrapper.EffectTiming == EffectTiming.NextTurn && trigger == ReactiveTrigger.StartOfTurn)
            {
                wrapper.EffectTiming = EffectTiming.Immediate;

                if (trigger != wrapper.ReactiveTrigger)
                {
                    // Move wrapper to correct reactive trigger
                    DictReactiveEffects[trigger].RemoveAt(i);
                    DictReactiveEffects[wrapper.ReactiveTrigger].Add(wrapper);
                    continue;
                }

                
                isRunningReactiveEffect = true;

                // run commands
                yield return wrapper.ExecuteCommands();
                DictReactiveEffects[trigger][i] = wrapper;

                isRunningReactiveEffect = false;
            }
        }
    }

    public void CheckTurnsReactiveEffects(ReactiveTrigger trigger)
    {
        Dictionary<ReactiveTrigger, List<ExecutableWrapper>> tempDict = DictReactiveEffects.ToDictionary(pair => pair.Key, pair => new List<ExecutableWrapper>(pair.Value));

        foreach (KeyValuePair<ReactiveTrigger, List<ExecutableWrapper>> pair in tempDict)
        {
            List<ExecutableWrapper> listWrapper = Extensions.CloneList(pair.Value);

            for (int i = 0; i < listWrapper.Count; i++)
            {
                ExecutableWrapper wrapper = listWrapper[i];

                if (trigger == ReactiveTrigger.StartOfTurn)
                {
                    if (wrapper.Turns > 1)
                    {
                        DictReactiveEffects[pair.Key][i].Turns--;
                    }
                    else if (wrapper.Turns == 1)
                    {
                        DictReactiveEffects[pair.Key].RemoveAt(i);
                        Debug.Log("Removed");
                    }
                }
                else if (wrapper.Turns == 0 && trigger == ReactiveTrigger.EndOfTurn)
                {
                    DictReactiveEffects[pair.Key].RemoveAt(i);
                }
            }
        }
    }

    #endregion

    public void UpdateStatsUI()
    {
        OnStatChanged?.Invoke();
    }

    public void AnimationEventAttack()
    {
        doDamage = true;
    }

    public void AnimationEventAttackFinish()
    {
        isAttackFinished = true;
    }
}
