using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using Action = System.Action;
using DG.Tweening;
using Random = UnityEngine.Random;
using Cinemachine;



public class Avatar : MonoBehaviour
{
    #region Public Variables

    [Header("Stats")]
    public bool AllowRootMotion = false;
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

    [SerializeField] List<SkinnedMeshRenderer> SkinnedMeshes;

    #endregion

    #region Bools
    public bool DoDamage { get; set; }
    public bool IsAttackFinished { get; set; }
    public bool IsHit { get; set; } = false;
    public bool IsRunningReactiveEffect { get; set; } = false;

    public bool IsInCounterState { get; set; } = false;
    public bool IsCountered { get; set; }
    public bool IsInStatusActivation { get; set; }
    public bool DoStatusDmg { get; set; }
    public bool IsRecoilDone { get; set; }

    #endregion

    #region Lists

    [HideInInspector] public Dictionary<ReactiveTrigger, List<EXEWrapper>> DictReactiveEffects = new();
    [HideInInspector] public List<StatusEffect> ListOfEffects = new();
    [HideInInspector] public List<GameAction> QueueGameActions = new();
    [HideInInspector] public List<Tween> CurrentActiveStatusEffectTween = new();

    #endregion

    #region Read Only

    public string Guid { get; private set; }
    public Animator Animator { get; protected set; }

    public WeaponData CurrentWeaponData { get; set; }

    public CinemachineVirtualCamera StatusCamera { get; private set; }

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


    protected void Awake()
    {
        Guid = System.Guid.NewGuid().ToString();
        StatusCamera = transform.GetComponentInChildren<CinemachineVirtualCamera>();


        if (SkinnedMeshes.Count == 0)
        {
            Debug.LogError($"{name} Avatar: Skinned Meshes is not assigned");
            return;
        }
    }

    private void Start()
    {
        // Random start point between 0 and 1 in the animation cycle
        Animator.Play("Basic Idle", 0, Random.Range(0f, 1f)); // Ref is grabbed in Player/Enemy Awake
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

        public void ResetBlock()
        {
            CurrentBlock = 0;
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
        // Create the reactive effect and check if it exists


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

        float extraDamage = 0;

        foreach (StatusEffect statusEffect in ListOfEffects)
        {
            if (statusEffect.Effect == Effect.GuardBroken)
            {
                StatusGuardBroken statusGuardBroken = statusEffect as StatusGuardBroken;
                extraDamage += damage * statusGuardBroken.ExtraDamagePercentage;
                continue;
            }
        }
        return damage + extraDamage;
    }

    public float ApplyBuffDmgCheck(float damage)
    {
        // Don't need to check
        if (ListOfEffects.Count == 0) return damage;

        float extraDamage = 0;

        foreach (StatusEffect statusEffect in ListOfEffects)
        {
            if (statusEffect.Effect == Effect.Amplify)
            {
                StatusAmplify status = statusEffect as StatusAmplify;
                extraDamage += damage * status.AttackIncreasePercentage;
                continue;
            }
        }

        return damage + extraDamage;
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

        List<EXEWrapper> overwriteQueue = new();
        List<EXEWrapper> stackQueue = new();

        List<EXEWrapper> listWrapper = Extensions.CloneList(DictReactiveEffects[trigger]);

        for (int i = 0; i < listWrapper.Count; i++)
        {
            EXEWrapper wrapper = listWrapper[i];

            if (IsGuardBroken() && wrapper.Commands.Any(command => command is AttackCommand)) continue; // NOTE: If we are guard broken & command is an attack command, ignore
                                                                                                        // (Maybe ignore every command if guard broken)
            #region Check Timing 

            if (wrapper.EffectTiming == EffectTiming.Immediate && trigger == wrapper.ReactiveTrigger)
            {
                // put in a queue to run effect later
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
                // If trigger is not Start of turn, skip
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
        
        List<EXEWrapper> sortedWrappers = SortQueue(overwriteQueue, stackQueue);

        foreach (EXEWrapper wrapper in sortedWrappers)
        {
            IsRunningReactiveEffect = true;

            EXEParameters.CardData = wrapper.CardData;

            ActionSequence actionSequence = new(wrapper.Commands);
            yield return actionSequence.Execute(null);

            IsRunningReactiveEffect = false;
        }

        #endregion
    }

    public void CheckTurnsReactiveEffects(ReactiveTrigger trigger)
    {
        bool isCounterActive = false;

        Dictionary<ReactiveTrigger, List<EXEWrapper>> tempDict = DictReactiveEffects.ToDictionary(pair => pair.Key, pair => new List<EXEWrapper>(pair.Value));

        foreach (KeyValuePair<ReactiveTrigger, List<EXEWrapper>> pair in tempDict)
        {
            List<EXEWrapper> listWrapper = Extensions.CloneList(pair.Value);

            for (int i = listWrapper.Count - 1; i >= 0; i--)
            {
                bool isWrapperRemoved = false;

                EXEWrapper wrapper = listWrapper[i];
                if (wrapper.OverwriteType == OverwriteType.Counter || wrapper.OverwriteType == OverwriteType.Counterattack)
                    isCounterActive = true;

                if (trigger == ReactiveTrigger.StartOfTurn)
                {
                    if (wrapper.Turns > 1) // Reduce turn count
                    {
                        DictReactiveEffects[pair.Key][i].Turns--;
                    }
                    else if (wrapper.Turns == 1) // Remove reactive effect
                    {
                        DictReactiveEffects[pair.Key].RemoveAt(i);
                        isWrapperRemoved = true;
                    }
                }
                else if (wrapper.Turns == 0 && trigger == ReactiveTrigger.EndOfTurn)
                {
                    DictReactiveEffects[pair.Key].RemoveAt(i);
                    isWrapperRemoved = true;
                }

                if (isWrapperRemoved) 
                    isCounterActive = false;
            }
        }

        if (!isCounterActive)
        {
            IsInCounterState = false;
            Animator.SetBool("isReady", false);
        }
    }

    public List<EXEWrapper> SortQueue(List<EXEWrapper> overwriteQueue, List<EXEWrapper> stackQueue)
    {
        List<EXEWrapper> sortedCommands = new();

        // Gets all enums of overwrite type in order (Counterattack being last)
        foreach (OverwriteType type in Enum.GetValues(typeof(OverwriteType))) // Only 1 type (Should play animations)
        {
            EXEWrapper wrapper = overwriteQueue.FirstOrDefault(w => w.OverwriteType == type);

            if (wrapper == null) continue;

            sortedCommands.Add(new EXEWrapper(wrapper.CardData, wrapper.Commands, wrapper.Effects));
        }

        foreach (StackType type in Enum.GetValues(typeof(StackType))) // Multiple types (Play no animations)
        {
            List<EXEWrapper> list = stackQueue.Where(w => w.StackType == type).ToList();

            if (list.Count == 0) continue;

            // Create new list if stack type is not damage, Skip this if do damage then add damage commands together with counterattack
            if (type == StackType.DoDamage && !overwriteQueue.Exists(w => w.OverwriteType == OverwriteType.Counterattack))
            {
                sortedCommands.Add(new EXEWrapper(list[0].CardData));
            }

            foreach (EXEWrapper wrapper in list)
            {
                sortedCommands[sortedCommands.Count - 1].Commands.AddRange(wrapper.Commands);
            }
        }

        return sortedCommands;
    }

    #endregion



    public void SetHealth(float health)
    {
        CurrentHealth = health;
    }


    public virtual void ResetDeath()
    {
        CurrentGuard = MaxGuard;
        CurrentBlock = 0;

        AllowRootMotion = false;


        Animator.Play("Basic Idle");
        Animator.ResetTrigger("TakeDamage");
        Animator.ResetTrigger("Counter");
        transform.localPosition = Vector3.zero;
        Animator.Update(0f);

        AllowRootMotion = true;
    }

    public virtual void PlayHurtSound() { }

    public virtual void PlayDeathSound() { }


    public Vector3 GetCharacterCenter()
    {
        Bounds bounds = SkinnedMeshes[0].bounds;

        for (int i = 1; i < SkinnedMeshes.Count; i++)
        {
            bounds.Encapsulate(SkinnedMeshes[i].bounds);
        }

        return bounds.center;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(GetCharacterCenter(), 0.1f);
    }
}
