using UnityEngine;

public class EscapeCommand : ICommand
{
    private int _escapeChance;
    private BattleStateManager _battleStateManager;

    public EscapeCommand(BattleStateManager battleStateManager)
    {
        _escapeChance = battleStateManager.getEscapeChance();
        _battleStateManager = battleStateManager;
    }

    public void Execute()
    {
        if (CalculateEscapeSuccess(_escapeChance))
        {
            Debug.Log("Escaped successfully");
            _battleStateManager.Flee();
        }

        Debug.Log("Escaped failed");
    }

    private bool CalculateEscapeSuccess(int areaEscapeChance)
    {
        return Random.Range(0, 100) > _escapeChance;
    }
}
