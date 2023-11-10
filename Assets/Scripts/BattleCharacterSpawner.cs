using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleCharacterSpawner : MonoBehaviour
{
    public GameObject EnemyBattleCharacterPrefab;
    public GameObject PartyMemberBattleCharacterPrefab;

    public Dictionary<Guid, PlayerMemberBattleCharacter> PlayerCharacters = new Dictionary<Guid, PlayerMemberBattleCharacter>();
    public Dictionary<Guid, BattleCharacter> EnemyCharacters = new Dictionary<Guid, BattleCharacter>();
    
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

    public void SetupCharacters(IEnumerable<BattleCharacterDTO> playerParty,
        List<CharacterStatusPanelController> playerPartyStatusPanels,
        IEnumerable<BattleCharacterDTO> enemies)
    {
        CreatePlayerParty(playerParty, playerPartyStatusPanels);
        CreateEnemies(enemies);
    }

    private void CreatePlayerParty(IEnumerable<BattleCharacterDTO> playerParty,
        List<CharacterStatusPanelController> playerPartyStatusPanels)
    {
        var characterNumber = 0;
        var verticalOffset = -5;
        var horizontalOffset = -5;

        foreach (var partyMember in playerParty)
        {
            playerPartyStatusPanels[characterNumber].Setup(partyMember.Name, (PlayerPartyStats)partyMember.Stats);

            var partyMemberGameObject = SpawnCharacter(horizontalOffset, verticalOffset, false);
            var partyMemberBattleCharacter = partyMemberGameObject.GetComponent<PlayerMemberBattleCharacter>();
            partyMemberBattleCharacter.SetupCharacter(
                partyMember.Id,
                partyMember.Name,
                partyMember.Stats,
                partyMember.Element,
                partyMember.ModelName);
            partyMemberBattleCharacter.SetStatusPanel(playerPartyStatusPanels[characterNumber]);
            PlayerCharacters.Add(partyMember.Id, partyMemberBattleCharacter);

            horizontalOffset += 5;
            characterNumber++;
        }
    }

    private void CreateEnemies(IEnumerable<BattleCharacterDTO> enemies)
    {
        var verticalOffset = 5;
        var horizontalOffset = -5;
        foreach (var enemy in enemies)
        {
            var enemyGameObject = SpawnCharacter(horizontalOffset, verticalOffset, true);
            var enemyBattleCharacter = enemyGameObject.GetComponent<BattleCharacter>();
            enemyBattleCharacter.SetupCharacter(
                enemy.Id,
                enemy.Name,
                enemy.Stats,
                enemy.Element,
                enemy.ModelName);
            EnemyCharacters.Add(enemy.Id, enemyBattleCharacter);

            horizontalOffset += 5;
        }
    }
}
