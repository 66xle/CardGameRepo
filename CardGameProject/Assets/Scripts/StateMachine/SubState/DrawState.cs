using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawState : CombatBaseState
{
    public DrawState(CombatStateMachine context, CombatStateFactory combatStateFactory, VariableScriptObject vso) : base(context, combatStateFactory, vso) { }

    public override void EnterState()
    {
        Debug.Log("Draw State");

        Player player = ctx.player;

        ctx.player.RecoverStamina();
        DrawCards(ctx.cardsToDraw);
    }

    public override void UpdateState()
    {
        CheckSwitchState();
    }

    public override void FixedUpdateState() { }
    public override void ExitState() { }
    public override void CheckSwitchState()
    {
        SwitchState(factory.Play());
    }
    public override void InitializeSubState() { }


    void DrawCards(int numberOfCards)
    {
        for (int i = 0; i < numberOfCards; i++)
        {
            if (ctx.playerDeck.Count <= 0)
            {
                // Reset deck and clear discard pile
                ctx.playerDeck = new List<Card>(ctx.discardPile);
                ctx.discardPile.Clear();

                // Shuffle deck
                Extensions.Shuffle(ctx.playerDeck);
            }

            // No more cards to draw
            if (ctx.playerDeck.Count <= 0)
                break;

            // Pick random card
            int index = Random.Range(0, ctx.playerDeck.Count);
            Card cardDrawed = ctx.playerDeck[index];

            ctx.CreateCard(cardDrawed, ctx.playerHand);
            ctx.playerDeck.Remove(cardDrawed);
        }
    }

    
}
