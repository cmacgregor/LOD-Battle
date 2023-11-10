using UnityEngine;

public class BattleEnemyTurnState : BattleBaseState
{
    private BattleCharacter _turnCharacter;

    public override void EnterState(BattleStateManager battleStateManager)
    {
        _turnCharacter = battleStateManager.GetActingCharacter();
        Debug.Log($"Entering {nameof(BattleEnemyTurnState)} for {_turnCharacter.Name}");
        //turn off player input

        //turn off player ui
        battleStateManager.HidePlayerUI();
    }

    public override void UpdateState(BattleStateManager battleStateManager)
    {
        //take enemy turn

        //pass for now
        battleStateManager.DetermineNextTurn();
    }
}
