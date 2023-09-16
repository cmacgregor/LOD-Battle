
using System;
using System.Collections.Generic;

public abstract class BattleCharacter
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public CharacterStats CombatStats { get; set; }
    public string ModelName { get; set; }
    public StatusAilments StatusAilment { get; set; }
    public int statusTurns { get; set; }
    public bool KOed { get; set; }
    public bool Defending { get; set; }


    public void Defend()
    {
        Defending = true;
        var postGuardHealth = CombatStats.CurrentHealth + (CombatStats.MaxHealth * 0.1);
        Math.Clamp(CombatStats.CurrentHealth, postGuardHealth, CombatStats.MaxHealth);
    }

    public void ApplyPreTurnStatusEffects()
    {
        if (StatusAilment == StatusAilments.None)
        {
            return;
        }
        if(StatusAilment == StatusAilments.Poison)
        {
            InflictPoisonDamage();
        }
    }

    public void ApplyPostTurnStatusEffects()
    {
        if (StatusAilment == StatusAilments.None)
        {
            return;
        }

        if (StatusAilment == StatusAilments.ArmBlock 
            || StatusAilment == StatusAilments.Stun 
            || StatusAilment == StatusAilments.Confusion)
        {
            if(statusTurns >= 3)
            {
                //TODO: show status disappation
                StatusAilment = StatusAilments.None;
            }
            else
            {
                statusTurns++;
            }
        }
    }

    private void InflictPoisonDamage()
    {
        int poisonDamage = (int)(CombatStats.MaxHealth * 0.1);
        TakeDamage(poisonDamage);
    }

    private void TakeDamage(int damage)
    {
        CombatStats.CurrentHealth = Math.Max(0, CombatStats.CurrentHealth - damage);

        if(CombatStats.CurrentHealth <= 0)
        {
            KOed = true;
        }
    }
}

public class EnemyBattleCharacter : BattleCharacter
{
    public bool CanCounterAttack;
    
    //these should be elsewhere as it doesn't come up in battle
    public int GoldReward;
    public int ExperienceReward;
    public Dictionary<string, int> ItemDrops;
}

public class PartyMemberBattleCharacter : BattleCharacter
{
    public PlayerPartyStats PartyStats;
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