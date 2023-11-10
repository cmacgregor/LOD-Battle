using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCharacterDTO
{
    public Guid Id;
    public string Name;
    public string ModelName;
    public string AnimatorControllerName;
    public CharacterBattleStats Stats;
    public ElementAlignment Element;
}
