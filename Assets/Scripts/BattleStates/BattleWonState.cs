using UnityEngine;

public class BattleWonState : BattleBaseState
{
    public override void EnterState(BattleStateManager battleStateManager)
    {
        Debug.Log($"Entering {nameof(BattleWonState)}");
        //turn off player input

        //turn off player ui
        battleStateManager.HidePlayerUI();
    }

    public override void UpdateState(BattleStateManager battleStateManager)
    {
        //play victory camera

        //wait for player input
    }
}
