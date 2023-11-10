using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleFleeingState : BattleBaseState
{
    public override void EnterState(BattleStateManager battleStateManager)
    {
        Debug.Log($"Entering {nameof(BattleFleeingState)}");
        //turn off player input

        //turn off player ui
        battleStateManager.HidePlayerUI();
    }

    public override void UpdateState(BattleStateManager battleStateManager)
    {
        //set all characters to fleeing

        //once that's finsished load post victory scene
    }
}
