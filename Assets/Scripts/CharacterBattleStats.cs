
public class CharacterBattleStats
{
    public int CurrentHealth;
    public int MaxHealth;
    public int Attack;
    public int Defense;
    public int MagicAttack;
    public int MagicDefense;
    public int Speed;
    public int AttackHitPercentage;
    public int MagicHitPercentage;
    public int AttackAvoidPercentage;
    public int MagicAvoidPercentage;
}

public class PlayerPartyStats : CharacterBattleStats
{
    public int CurrentMagic;
    public int MaxMagic;
    public int CurrentSpirit;
    public int MaxSpirit;
}

public enum ElementAlignment
{
    None,
    Fire,
    Darkness,
    Wind,
    Light,
    Earth,
    Thunder,
    Water,
}

public enum StatusAilments
{
    None,
    Poison,
    Stun,
    ArmBlock,
    Confusion,
    Bewitched,
    Fear,
    Despirit,
    Petrification,
    KO,
}