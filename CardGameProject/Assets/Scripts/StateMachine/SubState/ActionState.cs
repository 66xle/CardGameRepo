using events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System;
using DG.Tweening;

public class ActionState : CombatBaseState
{
    Avatar avatarPlayingCard;
    Avatar avatarOpponent;

    bool isInAction;
    bool isPlayingCard; // Playing Card animations
    bool hasAttacked; // Attack Animations

    public ActionState(CombatStateMachine context, CombatStateFactory combatStateFactory, VariableScriptObject vso) : base(context, combatStateFactory, vso) { }

    public override void EnterState()
    {
        Debug.Log("Action State");
        

        #region Decide Which Side Acts

        if (ctx.currentState.ToString() == PLAYERSTATE)
        {
            avatarPlayingCard = ctx.player;
            avatarOpponent = ctx.selectedEnemyToAttack;

            ctx.StartCoroutine(PlayCard(ctx.cardPlayed));
        }
        else
        {
            avatarPlayingCard = ctx.currentEnemyTurn;
            avatarOpponent = ctx.player;

            ctx.StartCoroutine(EnemyTurnPlayCard());
        }

        #endregion
    }
    public override void UpdateState()
    {
        CheckSwitchState();

        AnimationEvent();
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

    private IEnumerator EnemyTurnPlayCard()
    {
        // Play all enemy cards in queue
        foreach (Card cardPlayed in ctx.enemyCardQueue)
        {
            if (isInAction)
                yield return new WaitForEndOfFrame();
            else
                ctx.StartCoroutine(PlayCard(cardPlayed));
        }
    }

    private IEnumerator PlayCard(Card cardPlayed)
    {
        isInAction = true;
        hasAttacked = false;
        isPlayingCard = true;
        avatarPlayingCard.doDamage = false;
        avatarPlayingCard.attackFinished = false;

        ctx.displayCard.GetComponent<CardDisplay>().card = cardPlayed;
        ctx.displayCard.gameObject.SetActive(true);


        Animator avatarPlayingCardController = avatarPlayingCard.GetComponent<Animator>();
        Animator opponentController = avatarOpponent.GetComponent<Animator>();


        // Play Card Effect
        DetermineEffectTarget(cardPlayed);

        #region Deterimine card type

        if (cardPlayed.cardType == Type.Attack)
        {
            // Play Move Animation
            MoveAvatar();
            avatarPlayingCardController.SetTrigger("Move");

            if (avatarPlayingCard.gameObject.CompareTag("Player"))
            {
                ctx.followCam.LookAt = null;
                ctx.followCam.transform.rotation = ctx.defaultCam.transform.rotation;
                ctx.followCam.LookAt = avatarOpponent.transform;
                ctx.panCam.LookAt = avatarOpponent.transform;
                ctx.followCam.Priority = 30;
            }
                

            // Wait until opponent attacks
            yield return new WaitWhile(() => !hasAttacked);

            // Check counter
            if (avatarOpponent.isInCounterState)
            {
                opponentController.SetBool("isReady", false);
                opponentController.SetTrigger("Counter");

                avatarPlayingCardController.SetTrigger("Recoil");

                avatarOpponent.isInCounterState = false;
            }
            else
            {
                Attack(cardPlayed);
            }
        }
        else if (cardPlayed.cardType == Type.Counter)
        {
            avatarPlayingCard.isInCounterState = true;
            avatarPlayingCardController.SetBool("isReady", true);

            isPlayingCard = false;
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

        avatarPlayingCard.DisplayStats();
        avatarOpponent.DisplayStats();

        // Avatar returns to spot or finishes animation
        yield return new WaitWhile(() => isPlayingCard);

        // Attack finished
        ctx.displayCard.gameObject.SetActive(false);

        if (avatarPlayingCard.gameObject.CompareTag("Player"))
        {
            ctx.followCam.Priority = 10;
        }

        isInAction = false;
        Debug.Log("Finished Attacking");
    }



    #region Card Type

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
                ReduceHitToRecover();
            }
            else
            {
                ApplyGuardBroken();
            }
        }

        ctx.AvatarDeath(avatarOpponent, "Enemy");
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

    #endregion

    #region Card Actions

    public void ReduceHitToRecover()
    {
        for (int i = avatarOpponent.listOfEffects.Count - 1; i >= 0; i--)
        {
            if (avatarOpponent.listOfEffects[i].effect != Effect.GuardBroken)
                continue;

            avatarOpponent.listOfEffects[i].numberOfHitsToRecover--;
            if (avatarOpponent.listOfEffects[i].numberOfHitsToRecover <= 0)
            {
                avatarOpponent.RecoverGuardBreak();
                avatarOpponent.listOfEffects.RemoveAt(i);
            }
        }
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

    

    #endregion

    #region Status Effect

    private void DetermineEffectTarget(Card cardPlayed)
    {
        // Apply status effect on self and/or opponent

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


    #endregion

    #region Animation Related
    private void MoveAvatar()
    {
        // Determine position to move to
        Transform currentTransform = avatarPlayingCard.transform;
        Transform opponentTransform = avatarOpponent.transform;

        Vector3 currentPos = new Vector3(currentTransform.position.x, 0, currentTransform.position.z);
        Vector3 opponentPos = new Vector3(opponentTransform.position.x, 0, opponentTransform.position.z);

        Vector3 posToMove = opponentPos + opponentTransform.parent.transform.forward * 1.5f;

        currentTransform.DOMove(new Vector3(posToMove.x, currentTransform.position.y, posToMove.z), ctx.moveDuration).SetEase(ctx.moveAnimCurve).OnComplete(() =>
        {
            // Play attack animation
            avatarPlayingCard.GetComponent<Animator>().SetTrigger("Attack");

            if (avatarPlayingCard.gameObject.CompareTag("Player"))
            {
                ctx.panCam.transform.position = ctx.followCam.transform.position;
                ctx.panCam.transform.rotation = ctx.followCam.transform.rotation;
                ctx.panCam.Priority = 31;
            }
        });
    }

    private void ReturnAvatar()
    {
        // Determine position to move to
        Transform currentTransform = avatarPlayingCard.transform;
        Transform parentTransform = currentTransform.parent.transform;

        Vector3 currentPos = new Vector3(currentTransform.position.x, 0, currentTransform.position.z);
        Vector3 parentPos = new Vector3(parentTransform.position.x, 0, parentTransform.position.z);

        Vector3 posToMove = parentPos;

        currentTransform.DOMove(new Vector3(posToMove.x, currentTransform.position.y, posToMove.z), ctx.jumpDuration).SetEase(ctx.jumpAnimCurve).OnComplete(() =>
        {
            isPlayingCard = false;
        });
    }

    private void AnimationEvent()
    {
        if (avatarPlayingCard.doDamage)
        {
            avatarPlayingCard.doDamage = false;
            hasAttacked = true;
            
            if (!avatarOpponent.isInCounterState)
                avatarOpponent.GetComponent<Animator>().SetTrigger("TakeDamage");
        }

        // Move back to spot
        if (avatarPlayingCard.attackFinished)
        {
            avatarPlayingCard.attackFinished = false;
            ReturnAvatar();

            if (avatarPlayingCard.gameObject.CompareTag("Player"))
                ctx.panCam.Priority = 0;
        }
    }

    #endregion
}
