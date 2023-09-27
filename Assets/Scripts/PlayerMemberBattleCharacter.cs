
using System;

public class PlayerMemberBattleCharacter : BattleCharacter
{
    public int CurrentMagic;
    public int MaxMagic;
    public int CurrentSpirit;
    public int CurrentSpiritBars;
    public int MaxSpiritBars;

    public void SetupCharacter(Guid characterId, PlayerPartyStats stats, ElementAlignment element, string modelName)
    {
        Id = characterId;
        SetStats(stats);
        SetModel(modelName);
        Element = element;
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
}
