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
    public override void ExitState() {}
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

        yield return ctx.currentEnemyTurn.CheckReactiveEffects(ReactiveTrigger.EndOfTurn);
        ctx.currentEnemyTurn.CheckTurnsReactiveEffects(ReactiveTrigger.EndOfTurn);
    }

    private IEnumerator PlayCard(CardData cardData)
    {
        

        ExecutableParameters.ctx = ctx;
        ExecutableParameters.card = cardData.card;
        ExecutableParameters.weapon = cardData.weapon;
        ExecutableParameters.avatarPlayingCard = avatarPlayingCard;
        ExecutableParameters.avatarOpponent = avatarOpponent;

        isInAction = true;
        ctx.combatUIManager.ToggleHideUI(false);

        // Display Card
        //ctx.displayCard.GetComponent<CardDisplay>().card = cardPlayed;
        //ctx.displayCard.gameObject.SetActive(true);


        yield return ExecuteCommands(cardData.card.commands);



        isInAction = false;
        ctx.combatUIManager.ToggleHideUI(true);
        Debug.Log("Finished Attacking");

        ctx.enemyList.ForEach(enemy => enemy.isTakeDamage = false);
        ctx.player.isTakeDamage = false;


        if (avatarOpponent is Enemy && avatarOpponent.IsAvatarDead() || avatarPlayingCard is Enemy && avatarPlayingCard.IsAvatarDead())
        {
            ctx.EnemyDied();
        }
    }

    private IEnumerator ExecuteCommands(List<Executable> commands)
    {
        ActionSequence sequence = new ActionSequence(commands);
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
