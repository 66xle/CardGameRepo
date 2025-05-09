using System;

public class CombatStateFactory
{
    CombatStateMachine context;
    VariableScriptObject vso;

    public CombatStateFactory(CombatStateMachine currentContext, VariableScriptObject variableScriptableObject)
    {
        context = currentContext;
        vso = variableScriptableObject;
    }

    public CombatBaseState Player()
    {
        return new PlayerState(context, this, vso);
    }

    public CombatBaseState Enemy()
    {
        return new EnemyState(context, this, vso);
    }

    public CombatBaseState Draw()
    {
        return new DrawState(context, this, vso);
    }
    public CombatBaseState Play()
    {
        return new PlayState(context, this, vso);
    }

    public CombatBaseState Action()
    {
        return new ActionState(context, this, vso);
    }

    public CombatBaseState EnemyDraw()
    {
        return new EnemyDrawState(context, this, vso);
    }
    public CombatBaseState EnemyTurn()
    {
        return new EnemyTurnState(context, this, vso);
    }

    public CombatBaseState StatusEffect()
    {
        return new StatusEffectState(context, this, vso);
    }

    public CombatBaseState CombatEnd()
    {
        return new CombatEndState(context, this, vso);
    }
}
