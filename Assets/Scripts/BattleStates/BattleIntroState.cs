using UnityEngine;

public class BattleIntroState : BattleBaseState
{
    private BattleCamera _battleCamera;
    private BattleStateManager _battleStateManager;

    public override void EnterState(BattleStateManager battleStateManager)
    {
        _battleStateManager = battleStateManager;
        Debug.Log($"Entering {nameof(BattlePlayerTurnState)}");
        
        _battleCamera = _battleStateManager.GetComponent<BattleCamera>();
        //Turn off player input
        _battleStateManager.HidePlayerUI();
    }

    public override void UpdateState(BattleStateManager battleStateManager)
    {
        battleStateManager.DetermineNextTurn();
    }

    public void PlayBattleIntro()
    {
        _battleCamera.PerformBattleIntro(_battleStateManager.IsBossEncounter);
    }
}