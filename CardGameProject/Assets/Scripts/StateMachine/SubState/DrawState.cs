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
        DrawCards();
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


    void DrawCards()
    {
        CardManager cm = ctx.CardManager;

        for (int i = 0; i < cm.CardsToDraw; i++)
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

            ctx.CreateCard(cardDrawed, cm.PlayerHandTransform);
            cm.PlayerDeck.Remove(cardDrawed);
            cm.PlayerHand.Add(cardDrawed);
        }
    }

    
}
