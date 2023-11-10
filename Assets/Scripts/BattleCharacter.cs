using System;
using UnityEditor.Animations;
using UnityEngine;

public class BattleCharacter : MonoBehaviour 
{
    public Guid Id;
    public string Name;

    public CharacterSelectionIndicator Indicator;
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

    public AnimatorController AnimatorController;
    public AnimatorOverrideController AnimatorOverride;
    public GameObject CharacterModel;

    public void SetupCharacter(
        Guid characterId, 
        string name, 
        CharacterBattleStats stats, 
        ElementAlignment element, 
        string modelName)
    {
        Id = characterId;
        Name = name;
        SetStats(stats);
        SetModel(modelName);
        SetAnimationController(modelName);
        Element = element;
        Indicator.Hide();
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

        Indicator.SetIndicatorColor(CurrentHealth, MaxHealth);
    }

    public virtual void SetModel(string modelName)
    {
        modelName = modelName.ToLower();
        CharacterModel = Instantiate(Resources.Load($"Characters/{modelName}/{modelName}_model"),
            new Vector3(gameObject.transform.position.x, 0, gameObject.transform.position.z),
            Quaternion.Euler(-90f, 180, 0),
            gameObject.transform) as GameObject;
        CharacterModel.name = "CharacterModel";
    }

    public virtual void SetAnimationController(string modelName)
    {
        modelName = modelName.ToLower();

        Animator animator = CharacterModel.AddComponent<Animator>();

        AnimatorController = Resources.Load($"Characters/enemy_animationController") as AnimatorController;
        animator.runtimeAnimatorController = AnimatorController;

        AnimatorOverride = new AnimatorOverrideController(AnimatorController);
        AnimatorOverride.name = $"{modelName}_animatorOverride";
        AnimatorOverride.runtimeAnimatorController = AnimatorController;

        animator.runtimeAnimatorController = AnimatorOverride;

        var idleStateAnimationClip = Resources.Load($"Characters/{modelName}/{modelName}_idle") as AnimationClip;

        AnimatorOverride["Idle"] = idleStateAnimationClip;
        //List<KeyValuePair<AnimationClip, AnimationClip>> overrideClips = new List<KeyValuePair<AnimationClip, AnimationClip>>(AnimatorOverride.overridesCount)
        //{
        //    new KeyValuePair<AnimationClip, AnimationClip>(, idleStateAnimationClip)
        //};

        //AnimatorOverride.GetOverrides(overrideClips);
    }

    public void MoveToAttack(Vector3 attackPosition)
    {
        Debug.Log($"{Name} moving to attack character at {attackPosition}");

        //move to attack position
        CharacterModel.transform.position = attackPosition;
    }

    public Vector3 GetAttackPointPosition()
    {
        return AttackPoint.transform.position;
    }

    public void PlayAttackAnimation()
    {
        Debug.Log($"{Name} playing attack animation");
    }

    public void ReturnToDefaultPosition()
    {
        CharacterModel.transform.position = Vector3.zero;
    }

    public virtual void Defend()
    {
        Defending = true;
        var postGuardHealth = CurrentHealth + (int)(MaxHealth * 0.1);
        if(postGuardHealth >= MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }
        else
        {
            CurrentHealth = postGuardHealth;
        }

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
