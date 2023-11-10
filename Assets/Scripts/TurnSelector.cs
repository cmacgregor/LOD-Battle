
using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public class CharacterTP
{
    public readonly Guid CharacterId;
    public int Speed;
    public int TurnPriority;
}

public class TurnSelector
{
    private const int TURN_THRESHOLD = 217;
    private Guid _actingCharacterId;
    private Dictionary<Guid, CharacterTP> _turnPriorities = new Dictionary<Guid, CharacterTP>();
    
    public TurnSelector(Dictionary<Guid, int> characterSpeeds) {

        _turnPriorities = characterSpeeds.ToDictionary( x => x.Key, x => new CharacterTP() { Speed = x.Value, TurnPriority = 0 });
        UpdateCombatantTurnPriorites();
    }

    private void UpdateCombatantTurnPriorites()
    {
        foreach (var id in _turnPriorities.Keys.ToList())
        {
            if (id == _actingCharacterId)
            {
                _turnPriorities[id].TurnPriority = Math.Max(0, _turnPriorities[id].TurnPriority - TURN_THRESHOLD);
            }
            else
            {
                var characterSpeed = _turnPriorities[id].Speed;
                var lowRoll = characterSpeed - (characterSpeed/10);
                var  highRoll = characterSpeed + (characterSpeed/10);
                _turnPriorities[id].TurnPriority += Random.Range(lowRoll, highRoll);
            }
        }
    }

    public Guid DetermineNextActingCharacter()
    {
        _actingCharacterId = GetHighestTPCharacterId();
        UpdateCombatantTurnPriorites();

        return _actingCharacterId;
    }

    private Guid GetHighestTPCharacterId()
    {
        return _turnPriorities.Aggregate((tp1, tp2) => tp1.Value.TurnPriority > tp2.Value.TurnPriority ? tp1 : tp2).Key;
    }
}
