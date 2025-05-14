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
        foreach (CardData cardPlayed in ctx.cardManager.EnemyCardQueue)
        {
            yield return ctx.StartCoroutine(PlayCard(cardPlayed));
        }

        yield return ctx.currentEnemyTurn.CheckReactiveEffects(ReactiveTrigger.EndOfTurn);
        ctx.currentEnemyTurn.CheckTurnsReactiveEffects(ReactiveTrigger.EndOfTurn);
    }

    private IEnumerator PlayCard(CardData cardData)
    {
        ExecutableParameters.Ctx = ctx;
        ExecutableParameters.CardData = cardData;
        ExecutableParameters.WeaponData = cardData.Weapon;
        ExecutableParameters.AvatarPlayingCard = avatarPlayingCard;
        ExecutableParameters.AvatarOpponent = avatarOpponent;

        isInAction = true;
        ctx.combatUIManager.ToggleHideUI(false);

        // Display Card
        //ctx.displayCard.GetComponent<CardDisplay>().card = cardPlayed;
        //ctx.displayCard.gameObject.SetActive(true);


        yield return ExecuteCommands(cardData.Card.Commands);



        isInAction = false;
        ctx.combatUIManager.ToggleHideUI(true);

        ctx.enemyList.ForEach(enemy => enemy.IsTakeDamage = false);
        ctx.player.IsTakeDamage = false;

        Debug.Log("Finished Attacking");

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

    
}
