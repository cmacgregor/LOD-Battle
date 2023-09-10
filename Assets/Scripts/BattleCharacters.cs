
using System;
using System.Collections.Generic;

public class EnemyBattleCharacter
{
    public Guid Id;
    public string Name;
    public CharacterStats Stats;
    public int GoldReward;
    public int ExperienceReward;
    public bool CanCounterAttack;
    public Dictionary<string, int> ItemDrops;
}

public class PartyMemberBattleCharacter
{
    public Guid Id;
    public string Name;
    public PartyMemberStats Stats;
}
