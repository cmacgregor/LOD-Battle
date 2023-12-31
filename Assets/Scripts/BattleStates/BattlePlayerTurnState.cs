using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class BattlePlayerTurnState : BattleBaseState
{
    private BattleStateManager _battleStateManager;
    private BattleCharacter _turnCharacter;

    private TargetSelector _targetSelector;
    private Guid _attackTarget = Guid.Empty;

    private ICommand _escapeCommand;
    private ICommand _attackCommand;

    public override void EnterState(BattleStateManager battleStateManager)
    {
        _battleStateManager = battleStateManager;
        _turnCharacter = _battleStateManager.GetActingCharacter();
        Debug.Log($"Entering {nameof(BattlePlayerTurnState)} for {_turnCharacter.Name}");

        //should probably register event actions for current character so they're just called on button presses?

        _escapeCommand = new EscapeCommand(_battleStateManager);

        //Setup turn character player ui
        _battleStateManager.SetupPlayerActionsForPartyMember(_turnCharacter.Id);
        //Display character UI
        _battleStateManager.ShowPlayerUI();
    }

    public override void UpdateState(BattleStateManager battleStateManager)
    {
        if(_targetSelector != null)
        {
            _targetSelector.HandleInput();
        }
    }

    public void OnPlayerAttackButton()
    {
        Debug.Log($"Player attack selected with {_turnCharacter.Name}");
        _battleStateManager.HidePlayerUI();
        //TODO: check for confusion to change target list
        _targetSelector = new TargetSelector(_battleStateManager.GetEnemyIds(), _battleStateManager.GetEnemyIndicators());
        //yeild until we have a target?
    }

    public void OnPlayerDefendButton()
    {
        Debug.Log($"Defending with {_turnCharacter.Name}");

        _turnCharacter.Defend();
    }

    public void OnPlayerItemButton()
    {
        Debug.Log($"Using Item with {_turnCharacter.Name}");

        //TODO: Create item selection, target selection, and application workflow
    }

    public void OnPlayerEscapeButton()
    {
        Debug.Log($"Escaping with {_turnCharacter.Name}");
        //call escape command
        _escapeCommand.Execute();
    }
}