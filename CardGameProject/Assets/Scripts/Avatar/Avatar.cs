using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using Action = System.Action;
using MyBox;
using UnityEngine.VFX;
using DG.Tweening;



public class Avatar : MonoBehaviour
{
    #region Public Variables

    [Header("Stats")]
    public float MaxHealth = 100f;
    public int MaxGuard = 10;
    public float Attack;
    public float Defence;
    public float DefencePercentage;
    public float BlockScale;
    public ArmourType ArmourType;
    public DamageType DamageType;

    public Transform WeaponJoint;
    public Transform FollowJoint;
    public Transform RightHolder;

    #endregion

    #region Bools
    public bool DoDamage { get; set; }
    public bool IsAttackFinished { get; set; }
    public bool IsTakeDamage { get; set; } = false;
    public bool IsRunningReactiveEffect { get; set; } = false;

    public bool IsInCounterState { get; set; } = false;
    public bool IsInStatusActivation { get; set; }
    public bool DoStatusDmg { get; set; }

    #endregion

    #region Lists

    [HideInInspector] public Dictionary<ReactiveTrigger, List<ExecutableWrapper>> DictReactiveEffects = new();
    [HideInInspector] public List<StatusEffect> ListOfEffects = new();
    [HideInInspector] public List<GameAction> QueueGameActions = new();
    [HideInInspector] public List<Tween> CurrentActiveStatusEffectTween = new();

    #endregion

    #region Read Only

    public string Guid { get; private set; }
    public Animator Animator { get; private set; }

    #endregion

    #region Internal Variables

    protected event Action OnStatChanged;
    protected float _currentHealth;
    protected float _currentBlock;
    protected int _currentGuard;

    protected float CurrentHealth { get { return _currentHealth; } set { _currentHealth = value; UpdateStatsUI(); } }
    protected float CurrentBlock { get { return _currentBlock; } set { _currentBlock = value; UpdateStatsUI(); } }
    protected int CurrentGuard { get { return _currentGuard; } set { _currentGuard = value; UpdateStatsUI(); } }

    #endregion


    private void Awake()
    {
        Guid = System.Guid.NewGuid().ToString();
        Animator = GetComponent<Animator>();
    }

    private void LateUpdate()
    {
        AnimatorStateInfo state = Animator.GetCurrentAnimatorStateInfo(0);

        if (state.IsTag("Attack") && state.normalizedTime > 0.05f && !IsAttackFinished)
        {
            RightHolder.parent = WeaponJoint;
            //Animator.SetLayerWeight(1, 1);
        }
        else
        {
            RightHolder.parent = FollowJoint;
            //Animator.SetLayerWeight(1, 0);
        }
    }


    #region Avatar Methods

    #region Take Damage

        public virtual void TakeDamage(float damage) 
        {
            float block = CurrentBlock - damage;

            // Block is negative do damage / Block is positive dont do damage
            damage = block < 0 ? Mathf.Abs(block) : 0;

            if (block < 0) block = 0;

            CurrentBlock = block;

            float health = CurrentHealth - damage;
            CurrentHealth = Mathf.Clamp(health, 0, MaxHealth);
        }

        public void TakeDamageByStatusEffect(float damage)
        {
            float health = CurrentHealth - damage;
            CurrentHealth = Mathf.Clamp(health, 0, MaxHealth);
        }

        #endregion

        #region Guard

        public bool IsGuardReducible(DamageType damageType)
        {
            if (ArmourType == ArmourType.Light && damageType == DamageType.Slash ||
                ArmourType == ArmourType.Medium && damageType == DamageType.Pierce ||
                ArmourType == ArmourType.Heavy && damageType == DamageType.Blunt ||
                ArmourType == ArmourType.None)
            {
                return true;
            }

            return false;
        }

        public bool IsGuardBroken()
        {
            return CurrentGuard == 0 ? true : false;
        }

        public void ReduceGuard(int guardDamage)
        {
            int guard = CurrentGuard - guardDamage;
            CurrentGuard = Mathf.Clamp(guard, 0, MaxGuard);
        }

        public virtual void RecoverGuardBreak()
        {
            CurrentGuard = MaxGuard;
        }

        #endregion

        #region Other

        public bool IsAvatarDead()
        {
            if (CurrentHealth <= 0f)
            {
                return true;
            }

            return false;
        }

        public void AddBlock(float block)
        {
            CurrentBlock += block;
        }

        public void Heal(float healAmount)
        {
            CurrentHealth += healAmount;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
        }

        public void UpdateStatsUI()
        {
            OnStatChanged?.Invoke();
        }
    #endregion

    #endregion

    #region Apply Status Effects

    public void ApplyStatusEffect(StatusEffect statusEffect)
    {
        if (ListOfEffects.Any(status => status.Effect == statusEffect.Effect))
        {
            int index = ListOfEffects.FindIndex(status => status.Effect == statusEffect.Effect);
            ListOfEffects[index].OnApply(this);
        }
        else
        {
            ListOfEffects.Add(statusEffect);
            statusEffect.OnApply(this);
        }
        
        UpdateStatsUI();
    }

    #endregion

    #region Status Effects Methods

    public bool hasStatusEffect(Effect effect)
    {
        foreach (StatusEffect data in ListOfEffects)
        {
            if (data.Effect == effect)
            {
                return true;
            }
        }

        return false;
    }

    public float ApplyAdditionalDmgCheck(float damage)
    {
        // Don't need to check
        if (ListOfEffects.Count == 0) return damage;

        foreach (StatusEffect statusEffect in ListOfEffects)
        {
            if (statusEffect.Effect == Effect.GuardBroken)
            {
                StatusGuardBroken statusGuardBroken = statusEffect as StatusGuardBroken;
                return damage * statusGuardBroken.ExtraDamagePercentage + damage;
            }
        }

        return damage;
    }

    #endregion

    #region Game Action

    public bool IsGameActionInQueue<T>() where T : GameAction
    {
        return QueueGameActions.Any(gameAction => gameAction is T);
    }

    public GameAction GetGameActionFromQueue<T>() where T : GameAction
    {
        return QueueGameActions.First(gameAction => gameAction is T);
    }

    #endregion

    #region Reactive Effects

    public IEnumerator CheckReactiveEffects(ReactiveTrigger trigger)
    {
        if (!DictReactiveEffects.ContainsKey(trigger)) yield break;

        List<ExecutableWrapper> overwriteQueue = new();
        List<ExecutableWrapper> stackQueue = new();

        List<ExecutableWrapper> listWrapper = Extensions.CloneList(DictReactiveEffects[trigger]);

        for (int i = 0; i < listWrapper.Count; i++)
        {
            ExecutableWrapper wrapper = listWrapper[i];

            if (IsGuardBroken() && wrapper.Commands.Any(command => command is AttackCommand)) continue;

            #region Check Timing 

            if (wrapper.EffectTiming == EffectTiming.Immediate && trigger == wrapper.ReactiveTrigger)
            {
                // put in a queue
                if (wrapper.DuplicateEffect == DuplicateEffect.Overwrite)
                {
                    overwriteQueue.Add(wrapper);
                }
                else if (wrapper.DuplicateEffect == DuplicateEffect.Stack)
                {
                    stackQueue.Add(wrapper);
                }
            }
            else if (wrapper.EffectTiming == EffectTiming.NextTurn)
            {
                if (trigger != ReactiveTrigger.StartOfTurn) continue;

                wrapper.EffectTiming = EffectTiming.Immediate;

                if (trigger != wrapper.ReactiveTrigger)
                {
                    // Move wrapper to correct reactive trigger
                    DictReactiveEffects[trigger].RemoveAt(i);
                    DictReactiveEffects[wrapper.ReactiveTrigger].Add(wrapper);
                    continue;
                }

                DictReactiveEffects[trigger][i] = wrapper;

                if (wrapper.DuplicateEffect == DuplicateEffect.Overwrite)
                {
                    overwriteQueue.Add(wrapper);
                }
                else if (wrapper.DuplicateEffect == DuplicateEffect.Stack)
                {
                    stackQueue.Add(wrapper);
                }
            }

            #endregion
        }

        #region Sort and Run
        
        List<List<Executable>> sortedCommands = SortQueue(overwriteQueue, stackQueue);

        foreach (List<Executable> commands in sortedCommands)
        {
            IsRunningReactiveEffect = true;

            ActionSequence actionSequence = new(commands);
            yield return actionSequence.Execute(null);

            IsRunningReactiveEffect = false;
        }

        #endregion
    }

    public void CheckTurnsReactiveEffects(ReactiveTrigger trigger)
    {
        Dictionary<ReactiveTrigger, List<ExecutableWrapper>> tempDict = DictReactiveEffects.ToDictionary(pair => pair.Key, pair => new List<ExecutableWrapper>(pair.Value));

        foreach (KeyValuePair<ReactiveTrigger, List<ExecutableWrapper>> pair in tempDict)
        {
            List<ExecutableWrapper> listWrapper = Extensions.CloneList(pair.Value);

            for (int i = listWrapper.Count - 1; i >= 0; i--)
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

    public List<List<Executable>> SortQueue(List<ExecutableWrapper> overwriteQueue, List<ExecutableWrapper> stackQueue)
    {
        List<List<Executable>> sortedCommands = new();

        foreach (OverwriteType type in Enum.GetValues(typeof(OverwriteType)))
        {
            ExecutableWrapper wrapper = overwriteQueue.FirstOrDefault(w => w.OverwriteType == type);

            if (wrapper == null) continue;

            sortedCommands.Add(new List<Executable>(wrapper.Commands));
        }

        foreach (StackType type in Enum.GetValues(typeof(StackType)))
        {
            List<ExecutableWrapper> list = stackQueue.Where(w => w.StackType == type).ToList();

            if (list.Count == 0) continue;

            // Check to put any damage commands together with counterattack
            if (type == StackType.DoDamage && !overwriteQueue.Exists(w => w.OverwriteType == OverwriteType.Counterattack))
            {
                sortedCommands.Add(new List<Executable>());
            }

            foreach (ExecutableWrapper wrapper in list)
            {
                sortedCommands[sortedCommands.Count - 1].AddRange(wrapper.Commands);
            }
        }

        return sortedCommands;
    }

    #endregion

    

    #region Animation Events

    public void AnimationEventAttack()
    {
        DoDamage = true;
    }

    public void AnimationEventAttackFinish()
    {
        IsAttackFinished = true;
        Animator.SetBool("IsAttacking", false);
    }

    public void AnimationEventPlaySound()
    {
        AudioManager.Instance.PlayAudioType();
    }

    public void EnableWeaponTrail()
    {
        VisualEffect weaponTrail = RightHolder.GetComponentInChildren<VisualEffect>();
        weaponTrail.Play();

        Debug.Log("WEAPON TRAIL");
    }

    public void DisableWeaponTrail()
    {
        VisualEffect weaponTrail = RightHolder.GetComponentInChildren<VisualEffect>();
        weaponTrail.enabled = false;
    }

    #endregion

    public virtual void PlayHurtSound() { }

    public virtual void PlayDeathSound() { }
}
