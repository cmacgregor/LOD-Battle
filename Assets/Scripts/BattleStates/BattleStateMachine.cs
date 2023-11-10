using UnityEngine;

public abstract class BattleBaseState
{
    public abstract void EnterState(BattleStateManager battleStateManager);

    public abstract void UpdateState(BattleStateManager battleStateManager);

}
