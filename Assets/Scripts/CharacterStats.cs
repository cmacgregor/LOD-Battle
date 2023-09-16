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

public class CharacterStats
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

    public ElementAlignment Element;

    public void CharacterStatus(int currentHealth, int maxHealth, int attack, int defense, int magicAttack, int magicDefense, int speed,
        int attackHitPercentage, int magicHitPercentage, int attackAvoidPercentage, int magicAvoidPercentage,
        ElementAlignment element)
    { 
        CurrentHealth = currentHealth;
        MaxHealth = maxHealth;
        Attack = attack;
        Defense= defense;
        MagicAttack = magicAttack;
        MagicDefense = magicDefense;
        Speed = speed;
        AttackHitPercentage = attackHitPercentage;
        MagicHitPercentage = magicHitPercentage;
        AttackAvoidPercentage = attackAvoidPercentage;
        MagicAvoidPercentage = magicAvoidPercentage;
        Element = element;
    }
}

public class PlayerPartyStats
{
    public int CurrentMagic;
    public int MaxMagic;
    public int currentSpirit;
    public int currentSpiritBars;
    public int maxSpiritBars;
}