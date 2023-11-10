
using System;
using System.Linq;
using UnityEditor.Animations;
using UnityEngine;

public class PlayerMemberBattleCharacter : BattleCharacter
{
    public int CurrentMagic;
    public int MaxMagic;
    public int CurrentSpirit;
    public int CurrentSpiritBars;
    public int MaxSpiritBars;

    private CharacterStatusPanelController _statusPanelController;

    public void SetupCharacter(
        Guid characterId, 
        string name, 
        PlayerPartyStats stats, 
        ElementAlignment element, 
        string modelName)
    {
        Id = characterId;
        Name = name;
        SetStats(stats);
        SetModel(modelName);
        SetAnimationController(modelName);
        Element = element;
    }

    public void SetStatusPanel(CharacterStatusPanelController statusPanelController)
    {
        _statusPanelController = statusPanelController;
    }
    public void SetStats(PlayerPartyStats stats)
    {
        base.SetStats(stats);
        CurrentMagic = stats.CurrentMagic;
        MaxMagic = stats.MaxMagic;
        CurrentSpirit = stats.CurrentSpirit;
        CurrentSpiritBars = stats.CurrentSpiritBars;
        MaxSpiritBars = stats.MaxSpiritBars;
    }

    public override void SetModel(string modelName)
    {
        modelName = modelName.ToLower();
        CharacterModel = Instantiate(Resources.Load($"Characters/{modelName}/{modelName}_model"),
            new Vector3(gameObject.transform.position.x, 0, gameObject.transform.position.z),
            Quaternion.Euler(-90, 0, 0),
            gameObject.transform) as GameObject;
        CharacterModel.name = "CharacterModel";
    }

    public override void SetAnimationController(string modelName)
    {
        modelName = modelName.ToLower();
        AnimatorController = Resources.Load($"Characters/{modelName}/{modelName}_animationController") as AnimatorController;
        CharacterModel.AddComponent<Animator>().runtimeAnimatorController = AnimatorController;
    }

    public override void Defend()
    {
        base.Defend();
        _statusPanelController.CurrentHealth = CurrentHealth;
    }
}
