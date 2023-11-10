using System;

public interface IBattleCharacter : IModelled, IAnimated, IStats, IPartyStats
{
    public void SetupCharacter(Guid characterId, string name, CharacterBattleStats stats, ElementAlignment element, string modelName);
}

public interface IModelled
{
    public void SetModel(string modelName);
}

public interface IAnimated
{
    public void SetAnimationControllers(string animationControllerName);
}

public interface IStats
{
    public int CurrentHealth { get; set; }
    public int MaxHealth { get; set; }
    public int Attack { get; set; }
    public int Defense { get; set; }
    public int MagicAttack { get; set; }
    public int MagicDefense { get; set; }
    public int Speed { get; set; }
    public int AttackHitPercentage { get; set; }
    public int MagicHitPercentage { get; set; }
    public int AttackAvoidPercentage { get; set; }
    public int MagicAvoidPercentage { get; set; }

    //Consider splitting out into status
    public ElementAlignment Element { get; set; }
    public StatusAilments StatusAilment { get; set; }
    public int StatusTurns { get; set; }
    public bool Downed { get; set; }
    public bool Defending { get; set; }

    public void SetStats(CharacterBattleStats stats);
}

public interface IPartyStats
{
    public int CurrentMagic { get; set; }
    public int MaxMagic { get; set; }
    public int CurrentSpirit { get; set; }
    public int CurrentSpiritBars { get; set; }
    public int MaxSpiritBars { get; set; }
}


//consider scrapping
public interface ICharacterBattleActions
{ 
    public void Defend();
    public void TakeDamage(int damage);
}

public interface IAffectedByAilments
{
    public void ApplyPreTurnStatusEffects();
    public void ApplyPostTurnStatusEffects();
}