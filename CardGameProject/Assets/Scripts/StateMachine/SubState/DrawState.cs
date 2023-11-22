using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawState : CombatBaseState
{
    public DrawState(CombatStateMachine context, CombatStateFactory combatStateFactory, VariableScriptObject vso) : base(context, combatStateFactory, vso) { }

    public override void EnterState()
    {
        Debug.Log("Draw State");

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
        List<Card> tempList = ctx.cardsInDeck;

        for (int i = 0; i < numberOfCards; i++)
        {
            int index = Random.Range(0, tempList.Count);
            Card cardDrawed = tempList[index];

            ctx.CreateCard(cardDrawed);

            ctx.cardsInDeck.Remove(cardDrawed);
        }
    }
}
