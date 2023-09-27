using System;
using UnityEngine;

public class BattleCharacter : MonoBehaviour
{
    public Guid Id;
    public string Name;

    public CharacterSelectionIndicator Indicator;
    //TODO: add animation tree reference
    public GameObject CharacterModel;
    public GameObject AttackPoint;

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

    //Consider splitting out into status
    public ElementAlignment Element;
    public StatusAilments StatusAilment;
    public int StatusTurns;
    public bool Downed;
    public bool Defending;

    public void SetupCharacter(Guid characterId, CharacterBattleStats stats, ElementAlignment element, string modelName)
    {
        Id = characterId;
        SetStats(stats);
        SetModel(modelName);
        Element = element;
    }

    public void SetStats(CharacterBattleStats stats)
    {
        CurrentHealth = stats.CurrentHealth;
        MaxHealth = stats.MaxHealth;
        Attack = stats.Attack;
        Defense = stats.Defense;
        MagicAttack = stats.MagicAttack;
        MagicDefense = stats.MagicDefense;
        Speed = stats.Speed;
        AttackHitPercentage = stats.AttackHitPercentage;
        MagicHitPercentage = stats.MagicHitPercentage;
        AttackAvoidPercentage = stats.AttackAvoidPercentage;

        Indicator.SetIndicatorColor((decimal)CurrentHealth, (decimal)MaxHealth);
    }

    public void SetModel(string modelName)
    {
        Debug.Log($"Setting model to {modelName}");
        //TODO: Set model
    }


    public void AttackCharacter(Vector3 attackPosition)
    {
        //move to attack position

        //perform attack
    }

    public void Defend()
    {
        Defending = true;
        var postGuardHealth = CurrentHealth + (MaxHealth * 0.1);
        Math.Clamp(CurrentHealth, postGuardHealth, MaxHealth);
        Indicator.SetIndicatorColor((decimal)CurrentHealth, (decimal)MaxHealth);
        //TODO: Play defense animation
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth = Math.Max(0, CurrentHealth - damage);

        if (CurrentHealth <= 0)
        {
            Downed = true;
            //TODO: Play downed animation
        }

        Indicator.SetIndicatorColor((decimal)CurrentHealth, (decimal)MaxHealth);
    }

    public void ApplyPreTurnStatusEffects()
    {
        if (StatusAilment == StatusAilments.None)
        {
            return;
        }
        if (StatusAilment == StatusAilments.Poison)
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
            if (StatusTurns >= 3)
            {
                //TODO: show status disappation
                StatusAilment = StatusAilments.None;
            }
            else
            {
                StatusTurns++;
            }
        }
    }

    private void InflictPoisonDamage()
    {
        int poisonDamage = (int)(MaxHealth * 0.1);
        TakeDamage(poisonDamage);
    }

}
