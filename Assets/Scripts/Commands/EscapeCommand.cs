using UnityEngine;

public class EscapeCommand : ICommand
{
    private int _escapeChange;
    private BattleStateManager _battleStateManager;

    public EscapeCommand(int escapeChange, BattleStateManager battleStateManager)
    {
        _escapeChange = escapeChange;
        _battleStateManager = battleStateManager;
    }

    public void Execute()
    {
        if (CalculateEscapeSuccess(_escapeChange))
        {
            Debug.Log("Escaped successfully");
            _battleStateManager.Flee();
        }

        Debug.Log("Escaped failed");
    }

    private bool CalculateEscapeSuccess(int areaEscapeChance)
    {
        return Random.Range(0, 100) > _escapeChange;
    }
}
