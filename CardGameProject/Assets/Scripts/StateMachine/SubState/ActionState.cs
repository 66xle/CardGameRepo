using events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using DG.Tweening;
using System.Security.Cryptography;



public class ActionState : CombatBaseState
{
    Avatar avatarPlayingCard;
    Avatar avatarOpponent;

    bool isInAction;
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
    }   

    public override void FixedUpdateState() { }
    public override void ExitState() 
    {
        avatarPlayingCard.doDamage = false;
        avatarPlayingCard.isAttackFinished = false;
    }
    public override void CheckSwitchState()
    {
        if (!isInAction)
        {
            if (ctx.player.IsAvatarDead() || ctx.enemyList.Count == 0)
            {
                SwitchState(factory.CombatEnd());
            }
            else if (ctx.currentState.ToString() == PLAYERSTATE)
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
        foreach (CardData cardPlayed in ctx.cardManager.enemyCardQueue)
        {
            yield return ctx.StartCoroutine(PlayCard(cardPlayed));
        }
    }

    private IEnumerator PlayCard(CardData cardData)
    {
        avatarPlayingCard.doDamage = false;
        avatarPlayingCard.isAttackFinished = false;

        ExecutableParameters.ctx = ctx;
        ExecutableParameters.card = cardData.card;
        ExecutableParameters.weapon = cardData.weapon;
        ExecutableParameters.avatarPlayingCard = avatarPlayingCard;
        ExecutableParameters.avatarOpponent = avatarOpponent;

        ExecutableParameters.Targets = new List<Avatar>();
        ExecutableParameters.Queue = new List<Avatar>();

        WeaponData weapon = cardData.weapon;
        Card cardPlayed = cardData.card;

        isInAction = true;
        
        // Display Card
        //ctx.displayCard.GetComponent<CardDisplay>().card = cardPlayed;
        //ctx.displayCard.gameObject.SetActive(true);


        yield return ctx.StartCoroutine(ExecuteCommands(cardPlayed));
 

        // Attack finished
        //ctx.displayCard.gameObject.SetActive(false);

        

        isInAction = false;
        Debug.Log("Finished Attacking");

        if (ctx.currentState.ToString() == PLAYERSTATE && ctx.selectedEnemyToAttack.IsAvatarDead())
        {
            ctx.EnemyDied();
        }
    }

    private IEnumerator ExecuteCommands(Card card)
    {
        ActionSequence sequence = new ActionSequence();
        sequence.SetCommands(card.commands);

        yield return sequence.Execute(null);
    }



    #region Card Actions

    public void ReduceHitToRecover()
    {
        //for (int i = avatarOpponent.listOfEffects.Count - 1; i >= 0; i--)
        //{
        //    if (avatarOpponent.listOfEffects[i].effect != Effect.GuardBroken)
        //        continue;

        //    avatarOpponent.listOfEffects[i].numberOfHitsToRecover--;
        //    if (avatarOpponent.listOfEffects[i].numberOfHitsToRecover <= 0)
        //    {
        //        avatarOpponent.RecoverGuardBreak();
        //        avatarOpponent.listOfEffects.RemoveAt(i);
        //    }
        //}
    }

    #endregion


    
}
