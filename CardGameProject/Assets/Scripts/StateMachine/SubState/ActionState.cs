using events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

public class ActionState : CombatBaseState
{
    Avatar avatarPlayingCard;
    Avatar avatarOpponent;
    bool isInAction;

    public ActionState(CombatStateMachine context, CombatStateFactory combatStateFactory, VariableScriptObject vso) : base(context, combatStateFactory, vso) { }

    public override void EnterState()
    {
        Debug.Log("Action State");

        // Attack started
        isInAction = true;

        #region Decide Which Side Acts

        if (ctx.currentState.ToString() == PLAYERSTATE)
        {
            avatarPlayingCard = ctx.player;
            avatarOpponent = ctx.selectedEnemyToAttack;

            PlayCard(ctx.cardPlayed);
        }
        else
        {
            avatarPlayingCard = ctx.currentEnemyTurn;
            avatarOpponent = ctx.player;

            // Play all enemy cards in queue
            foreach (Card cardPlayed in ctx.enemyCardQueue)
            {
                PlayCard(cardPlayed);
            }
        }

        #endregion
    }
    public override void UpdateState()
    {
        CheckSwitchState();
    }   

    public override void FixedUpdateState() { }
    public override void ExitState() { }
    public override void CheckSwitchState()
    {
        if (!isInAction)
        {
            if (ctx.currentState.ToString() == PLAYERSTATE)
            {
                SwitchState(factory.Play());
            }
            else
            {
                SwitchState(factory.EnemyTurn());
            }
        }
    }
    public override void InitializeSubState() { }

    private async void PlayCard(Card cardPlayed)
    {
        ctx.displayCard.GetComponent<CardDisplay>().card = cardPlayed;
        ctx.displayCard.gameObject.SetActive(true);

        await Task.Delay(1000); // Need to test if game is paused

        DetermineEffectTarget(cardPlayed);

        #region Deterimine card type

        if (cardPlayed.cardType == Type.Attack)
        {
            Attack(cardPlayed);
        }
        else if (cardPlayed.cardType == Type.Defend)
        {
            Defend(cardPlayed);
        }
        else if (cardPlayed.cardType == Type.Heal)
        {
            Heal(cardPlayed);
        }

        #endregion

        ctx.displayCard.gameObject.SetActive(false);

        avatarPlayingCard.DisplayStats();
        avatarOpponent.DisplayStats();

        // Attack finished
        isInAction = false;
        Debug.Log("Finished Attacking");
    }

    private void Attack(Card cardPlayed)
    {
        float damage = cardPlayed.value;
        avatarOpponent.TakeDamage(damage);

        ReduceGuard();

        // Apply effect when guard is broken
        if (avatarOpponent.isGuardBroken())
        {
            // Check if avatar has guard broken effect
            if (avatarOpponent.hasStatusEffect(Effect.GuardBroken))
            {
                avatarOpponent.ReduceHitToRecover();
            }
            else
            {
                ApplyGuardBroken();
            }
        }

        // Check deaths
        if (ctx.currentState.ToString() == PLAYERSTATE)
        {
            EnemiesAlive();
        }
        else
        {
            // Check player Death
        }
    }

    private void Defend(Card cardPlayed)
    {
        float block = cardPlayed.value;

        avatarPlayingCard.AddBlock(block);
    }

    private void Heal(Card cardPlayed)
    {
        float healAmount = cardPlayed.value;

        avatarPlayingCard.Heal(healAmount);
    }

    private void ReduceGuard()
    {
        if (avatarOpponent.armourType == ArmourType.Light && avatarPlayingCard.damageType == DamageType.Slash ||
            avatarOpponent.armourType == ArmourType.Medium && avatarPlayingCard.damageType == DamageType.Pierce ||
            avatarOpponent.armourType == ArmourType.Heavy && avatarPlayingCard.damageType == DamageType.Blunt ||
            avatarOpponent.armourType == ArmourType.None)
        {
            avatarOpponent.ReduceGuard();
        }
    }

    private void ApplyGuardBroken()
    {
        if (avatarOpponent.armourType == ArmourType.Light || avatarOpponent.armourType == ArmourType.None) avatarOpponent.ApplyGuardBreak(ctx.guardBreakLightArmour);
        else if (avatarOpponent.armourType == ArmourType.Medium) avatarOpponent.ApplyGuardBreak(ctx.guardBreakMediumArmour);
        else if (avatarOpponent.armourType == ArmourType.Heavy) avatarOpponent.ApplyGuardBreak(ctx.guardBreakHeavyArmour);
    }

    private void EnemiesAlive()
    {
        // Check if enemy is dead
        if (ctx.selectedEnemyToAttack.isDead())
        {
            // Remove and destroy enemy
            ctx.enemyList.Remove(ctx.selectedEnemyToAttack);
            ctx.DestroyEnemy(ctx.selectedEnemyToAttack);

            // Are there enemies still alive
            if (ctx.enemyList.Count > 0)
            {
                // Select different enemy
                ctx.selectedEnemyToAttack = ctx.enemyList[0].GetComponent<Enemy>();
                ctx.selectedEnemyToAttack.GetComponent<MeshRenderer>().material = ctx.redMat;
            }
            else if (ctx.enemyList.Count == 0) // No enemies
            {
                ctx.ClearCombatScene();

                ctx.eventDisplay.FinishCombatEvent();
            }
        }
    }


    private void DetermineEffectTarget(Card cardPlayed)
    {
        foreach (StatusEffect effect in cardPlayed.selfEffects)
        {
            ApplyEffects(effect, avatarPlayingCard);
        }

        foreach (StatusEffect effect in cardPlayed.applyEffects)
        {
            ApplyEffects(effect, avatarOpponent);
        }
    }

    private void ApplyEffects(StatusEffect effectObj, Avatar targetAvatar)
    {
        Effect effect = effectObj.effect;

        if (effect == Effect.Bleed)
        {
            targetAvatar.ApplyBleed(effectObj);
        }
    }
}
