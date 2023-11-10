using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetSelector
{
    private IList<Guid> _targets;
    private IDictionary<Guid, CharacterSelectionIndicator> _targetIndicators;
    private Guid _selectedTarget;
    private int _targetedIndex;

    public Guid Target;

    public TargetSelector(IList<Guid> targets, IDictionary<Guid, CharacterSelectionIndicator> targetIndicators)
    {
        _targets = targets;
        _targetIndicators = targetIndicators;
        _selectedTarget = targets.First();
    }
    
    public void HandleInput()
    {
        _targetIndicators[_selectedTarget].Show();
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.W))
        {
            CycleTargetLeft();
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.S))
        {
            CycleTargetRight();
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            Target = _targets[_targetedIndex];
        }
    }

    private void CycleTargetLeft()
    {
        _targetIndicators[_selectedTarget].Hide();

        _targetedIndex--;
        if (_targetedIndex < 0)
        {
            _targetedIndex = _targets.Count() - 1;
        }

        _selectedTarget = _targets[_targetedIndex];
        _targetIndicators[_selectedTarget].Show();
    }

    private void CycleTargetRight()
    {
        _targetIndicators[_selectedTarget].Hide();

        _targetedIndex++;
        if (_targetedIndex >= _targets.Count())
        {
            _targetedIndex = 0;
        }

        _selectedTarget = _targets[_targetedIndex];
        _targetIndicators[_selectedTarget].Show();
    }
}