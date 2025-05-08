using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawState : CombatBaseState
{
    public DrawState(CombatStateMachine context, CombatStateFactory combatStateFactory, VariableScriptObject vso) : base(context, combatStateFactory, vso) { }

    public override void EnterState()
    {
        Debug.Log("Draw State");

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
        CardManager cm = ctx.cardManager;

        for (int i = 0; i < numberOfCards; i++)
        {
            if (cm.PlayerDeck.Count <= 0)
            {
                // Reset deck and clear discard pile
                cm.PlayerDeck = new List<CardData>(cm.DiscardPile);
                cm.DiscardPile.Clear();

                // Shuffle deck
                Extensions.Shuffle(cm.PlayerDeck);
            }

            // No more cards to draw
            if (cm.PlayerDeck.Count <= 0)
                break;

            // Pick random card
            int index = Random.Range(0, cm.PlayerDeck.Count);
            CardData cardDrawed = cm.PlayerDeck[index];

            ctx.CreateCard(cardDrawed, ctx.playerHandTransform);
            cm.PlayerDeck.Remove(cardDrawed);
            cm.PlayerHand.Add(cardDrawed);
        }
    }

    
}
