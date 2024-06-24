using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CombatBaseState
{
    protected bool isRootState = false;
    protected CombatStateMachine ctx;
    protected CombatStateFactory factory;
    protected VariableScriptObject vso;
    protected CombatBaseState currentSuperState;
    public CombatBaseState currentSubState; // CHANGE TO PROTECTED LATER

    public CombatBaseState(CombatStateMachine context, CombatStateFactory factory, VariableScriptObject vso)
    {
        this.ctx = context;
        this.factory = factory;
        this.vso = vso;
    }

    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void FixedUpdateState();
    public abstract void ExitState();
    public abstract void CheckSwitchState();
    public abstract void InitializeSubState();

    public void UpdateStates()
    {
        UpdateState();
        if (currentSubState != null)
        {
            currentSubState.UpdateState();
        }
    }

    public void FixedUpdateStates()
    {
        FixedUpdateState();
        if (currentSubState != null)
        {
            currentSubState.FixedUpdateState();
        }
    }

    protected void SwitchState(CombatBaseState newState)
    {
        ExitState();

        if (isRootState)
            ctx.currentState = newState;

        newState.EnterState();

        if (isRootState)
        {
            // new root state, substate is null
            if (currentSubState != null && newState.currentSubState != null)
            {
                // Exit roots substates
                if (currentSubState.ToString() != newState.currentSubState.ToString())
                {
                    currentSubState.ExitState();
                    currentSubState = newState.currentSubState;
                }
            }

            
        }
        else if (currentSuperState != null)
        {
            currentSuperState.SetSubState(newState);
        }
    }
    protected void SetSuperState(CombatBaseState newSuperState)
    {
        currentSuperState = newSuperState;
    }
    protected void SetSubState(CombatBaseState newSubState)
    {
        // Sets the supers sub state
        currentSubState = newSubState;

        // Set the sub's super state
        newSubState.SetSuperState(this);
    }
}

    
