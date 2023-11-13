using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatStateMachine : MonoBehaviour
{
    [SerializeField] string debugState;
    [SerializeField] string subState;

    [Space]

    // Card Variables
    public Transform playerHand;

    [HideInInspector]
    public VariableScriptObject vso; // Not using for now

    public CombatBaseState currentState;
    private CombatStateFactory states;


    // Start is called before the first frame update
    void Start()
    {
        states = new CombatStateFactory(this, vso);
        currentState = new PlayerState(this, states, vso);
        currentState.EnterState();
    }

    // Update is called once per frame
    void Update()
    {
        currentState.UpdateStates();
        if (currentState.currentSubState != null)
        {
            subState = currentState.currentSubState.ToString();
        }
        debugState = currentState.ToString();
    }
}
