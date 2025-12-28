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
            avatarOpponent = ctx._selectedEnemyToAttack;

            ctx.StartCoroutine(PlayCard(ctx._cardPlayed));
        }
        else
        {
            avatarPlayingCard = ctx.CurrentEnemyTurn;
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
            if (ctx.player.IsAvatarDead() || ctx.EnemyList.Count == 0)
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
        foreach (CardData cardPlayed in ctx.CardManager.EnemyCardQueue)
        {
            yield return ctx.StartCoroutine(PlayCard(cardPlayed));
        }

        yield return ctx.CurrentEnemyTurn.CheckReactiveEffects(ReactiveTrigger.EndOfTurn);
        ctx.CurrentEnemyTurn.CheckTurnsReactiveEffects(ReactiveTrigger.EndOfTurn);
    }

    private IEnumerator PlayCard(CardData cardData)
    {
        EXEParameters.Ctx = ctx;
        EXEParameters.CardData = cardData;
        EXEParameters.AvatarPlayingCard = avatarPlayingCard;
        EXEParameters.AvatarOpponent = avatarOpponent;

        if (cardData.Gear is WeaponData)
            avatarPlayingCard.CurrentWeaponData = (WeaponData)cardData.Gear;

        isInAction = true;
        //ctx.CombatUIManager.HideGameplayUI(true);

        // Display Card
        //ctx.displayCard.GetComponent<CardDisplay>().card = cardPlayed;
        //ctx.displayCard.gameObject.SetActive(true);


        yield return ExecuteCommands(cardData.Card.Commands);

        isInAction = false;

        ctx.EnemyList.ForEach(enemy => enemy.IsHit = false);
        ctx.player.IsHit = false;

        Debug.Log("Finished Attacking");

        //if (avatarPlayingCard is Player)
        //    ctx.CombatUIManager.ToggleHideUI(true);

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
