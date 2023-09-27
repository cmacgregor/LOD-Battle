using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleCharacterSpawner : MonoBehaviour
{
    public GameObject EnemyBattleCharacterPrefab;
    public GameObject PartyMemberBattleCharacterPrefab;

    //TODO: Auto space spawns based on the number of characters instead of predefining positions
    public GameObject SpawnCharacter(int horizontalOffset, int verticalOffset, bool isEnemy)
    {
        GameObject spawn = null;
        
        if (isEnemy)
        {
            spawn = Instantiate(EnemyBattleCharacterPrefab, new Vector3(horizontalOffset, 1, verticalOffset), Quaternion.identity);
        }
        else
        {
            spawn = Instantiate(PartyMemberBattleCharacterPrefab, new Vector3(horizontalOffset, 1, verticalOffset), Quaternion.Euler(0, 180f, 0));
        }

        return spawn;
    }

}
