
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


public class BattleSystemDataContext
{
    public Dictionary<Guid, CharacterStatusPanelController> PartyStatusPanels = new Dictionary<Guid, CharacterStatusPanelController>();

    public Dictionary<Guid, int> TurnPriorities;
    public Dictionary<Guid, CharacterSelectionIndicator> CharacterSelectionIndicators;
    public Dictionary<Guid, BattleCharacter> BattleCharacters;
    public Dictionary<Guid, GameObject> CharacterGameObjects;

    public IList<Guid> PlayerPartyIds;
    public IList<Guid> EnemyPartyIds;

    private const int TURN_THRESHOLD = 217;
    private const decimal HIGH_HEALTH_PERCENTAGE = 2m/3m;
    private const decimal LOW_HEALTH_PERCENTAGE = 1m/3m;

    public BattleSystemDataContext()
    {
        PartyStatusPanels = new Dictionary<Guid, CharacterStatusPanelController>();

        TurnPriorities = new Dictionary<Guid, int>();
        CharacterSelectionIndicators = new Dictionary<Guid, CharacterSelectionIndicator>();
        BattleCharacters = new Dictionary<Guid, BattleCharacter>();
        CharacterGameObjects = new Dictionary<Guid, GameObject>();

        PlayerPartyIds = new List<Guid>();
        EnemyPartyIds = new List<Guid>();
    }

    public Guid GetHighestTPCharacterId()
    {
        return TurnPriorities.Aggregate((tp1, tp2) => tp1.Value > tp2.Value ? tp1 : tp2).Key;
    }

    public void UpdateCombatantTurnPriorites(Guid turnCharacterId)
    {
        foreach (var id in TurnPriorities.Keys.ToList())
        {
            if (id == turnCharacterId)
            {
                TurnPriorities[id] = Math.Max(0, TurnPriorities[id] - TURN_THRESHOLD);
            }
            else
            {
                var characterSpeed = BattleCharacters[id].CombatStats.Speed;
                var lowRoll = (int)(characterSpeed - (characterSpeed * 0.1));
                var highRoll = (int)(characterSpeed + (characterSpeed * 0.1));
                TurnPriorities[id] += (int)Random.Range(lowRoll, highRoll);
            }
        }
    }

    public bool IsPartyMember(Guid checkId)
    {
        return PlayerPartyIds.Contains(checkId);
    }

    public void SetCharacterHealthIndicatorColor(Guid id)
    {
        var currentHealth = BattleCharacters[id].CombatStats.CurrentHealth;
        var maxHealth = BattleCharacters[id].CombatStats.MaxHealth;
        var healthPercentage = (decimal)currentHealth / (decimal)maxHealth;

        if(healthPercentage >= HIGH_HEALTH_PERCENTAGE) 
        {
            CharacterSelectionIndicators[id].SetColorToHighHealthColor();
        }
        else if (healthPercentage < HIGH_HEALTH_PERCENTAGE && healthPercentage > LOW_HEALTH_PERCENTAGE)
        {
            CharacterSelectionIndicators[id].SetColorToMidHealthColor();
        }
        else if(healthPercentage <= LOW_HEALTH_PERCENTAGE)
        {
            CharacterSelectionIndicators[id].SetColorToLowHealthColor();
        }
    }

    public void CharacterAttacks(Guid id)
    {
        Debug.Log($"Attacking with {BattleCharacters[id].Name}");

        //TODO: Perform targe selection

        //TODO: Perform run up animation

        //TODO: Attack

    }
    public void CharacterDefends(Guid id)
    {
        Debug.Log($"Defending with {BattleCharacters[id].Name}");
     
        BattleCharacters[id].Defend();
        PartyStatusPanels[id].CurrentHealth = BattleCharacters[id].CombatStats.CurrentHealth;

        //TODO: Play defense animation
    }
}
