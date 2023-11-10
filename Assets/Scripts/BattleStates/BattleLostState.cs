using UnityEngine;

public class BattleLostState : BattleBaseState
{
    public override void EnterState(BattleStateManager battleStateManager)
    {
        Debug.Log($"Entering {nameof(BattleLostState)}");
        //turn off player input

        //turn off player ui

        //load post victory screen 
    }

    public override void UpdateState(BattleStateManager battleStateManager)
    {
        //play loss camera

        //wait for player input

        //load gameover scene
    }
}
