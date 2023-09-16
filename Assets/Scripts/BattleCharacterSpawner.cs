using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleCharacterSpawner : MonoBehaviour
{
    public GameObject BattleCharacterPrefab;

    private const int _playerLineUpOffset = -5;
    private const int _enemyLineUpOffset = 5;

    public Dictionary<Guid, GameObject> SpawnCharacterLine(IList<Tuple<Guid, string>> characterTuples, bool isEnemies)
    {
        var characters = new Dictionary<Guid, GameObject>();
        var characterHorizontalOffset = -5;
        var characterVerticalOffset = isEnemies ? _enemyLineUpOffset : _playerLineUpOffset;

        foreach (var character in characterTuples) 
        {
            characters.Add(character.Item1, SpawnCharacter(character.Item2, new Vector3(characterHorizontalOffset, 1, characterVerticalOffset), isEnemies));
            
            characterHorizontalOffset += 5;
        }

        return characters;
    }

    //TODO: Auto space spawns based on the number of characters instead of predefining positions
    //TODO: swap in model 
    private GameObject SpawnCharacter(string modelName, Vector3 position, bool isEnemy)
    {
        var spawn = Instantiate(BattleCharacterPrefab, position, Quaternion.identity);
        if (isEnemy)
        {
            spawn.GetComponent<Renderer>().material = Resources.Load<Material>("Materials\\Enemy");
        }
        return spawn;
    }

}
