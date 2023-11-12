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
}
