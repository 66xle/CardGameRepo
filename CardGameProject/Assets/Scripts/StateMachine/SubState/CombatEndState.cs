using System.Collections;
using UnityEngine;

public class CombatEndState : CombatBaseState
{
    public CombatEndState(CombatStateMachine context, CombatStateFactory combatStateFactory, VariableScriptObject vso) : base(context, combatStateFactory, vso) { }

    public override void EnterState()
    {
        Debug.Log("Combat End State");

        if (ctx.player.IsAvatarDead())
        {
            ctx.StartCoroutine(OpenGameOverUI());
        }
        else
        {
            ctx.StartCoroutine(OpenRewardUI());
        }
    }

    public override void UpdateState()
    {
        CheckSwitchState();
    }

    public override void FixedUpdateState() { }
    public override void ExitState() { }
    public override void CheckSwitchState()
    {
        
    }
    public override void InitializeSubState() { }

    public IEnumerator OpenRewardUI()
    {
        yield return new WaitForSeconds(1f);

        ctx.RewardManager.DisplayReward();
        Time.timeScale = 0;
    }

    public IEnumerator OpenGameOverUI()
    {
        yield return new WaitForSeconds(1f);

        AudioManager.Instance.PlaySound(AudioType.UIGameOver);

        ctx.RewardManager.GameOverUI.SetActive(true);
        Time.timeScale = 0;
    }
}
