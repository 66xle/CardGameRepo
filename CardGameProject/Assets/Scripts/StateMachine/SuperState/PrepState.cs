using UnityEngine;

public class PrepState : CombatBaseState
{
    public PrepState(CombatStateMachine context, CombatStateFactory combatStateFactory, VariableScriptObject vso) : base(context, combatStateFactory, vso) 
    {
        isRootState = true;

        ctx.currentSuperState = this.ToString(); // temp debugging
    }

    public override void EnterState()
    {
        Debug.Log("Prep State");

        ctx._isInPrepState = true;
    }

    public override void UpdateState()
    {
        CheckSwitchState();
    }

    public override void FixedUpdateState() { }
    public override void ExitState() { }

    public override void CheckSwitchState()
    {
        if (!ctx._isInPrepState)
        {
            SwitchState(factory.Player());
        }
    }
    public override void InitializeSubState() { }
}
