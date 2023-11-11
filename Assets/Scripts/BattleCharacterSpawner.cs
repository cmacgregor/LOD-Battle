using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleCharacterSpawner : MonoBehaviour
{
    public GameObject EnemyBattleCharacterPrefab;
    public GameObject PartyMemberBattleCharacterPrefab;

    public GameObject PlayerCenterCharacterStatusPanel;
    public GameObject PlayerLeftCharacterStatusPanel;
    public GameObject PlayerRightCharacterStatusPanel;
    
    public Dictionary<Guid, PlayerMemberBattleCharacter> SetupPartyCharacters(IEnumerable<BattleCharacterDTO> playerParty)
    {
        var playerCharacters = new Dictionary<Guid, PlayerMemberBattleCharacter>();

        var playerPartyStatusPanels = new List<CharacterStatusPanelController>
        {
            PlayerLeftCharacterStatusPanel.GetComponent<CharacterStatusPanelController>(),
            PlayerCenterCharacterStatusPanel.GetComponent<CharacterStatusPanelController>(),
            PlayerRightCharacterStatusPanel.GetComponent<CharacterStatusPanelController>(),
        };

        var characterNumber = 0;
        var verticalOffset = -5;
        var horizontalOffset = -5;

        foreach (var partyMember in playerParty)
        {
            var partyMemberGameObject = SpawnCharacter(horizontalOffset, verticalOffset, false);
            var partyMemberBattleCharacter = partyMemberGameObject.GetComponent<PlayerMemberBattleCharacter>();
            partyMemberBattleCharacter.SetupCharacter(
                partyMember.Id,
                partyMember.Name,
                partyMember.Stats,
                partyMember.Element,
                partyMember.ModelName);
            playerPartyStatusPanels[characterNumber].Setup(partyMemberBattleCharacter);

            playerCharacters.Add(partyMember.Id, partyMemberBattleCharacter);

            horizontalOffset += 5;
            characterNumber++;
        }

        return playerCharacters;
    }

    public Dictionary<Guid, BattleCharacter> SetupEnemyCharacters(IEnumerable<BattleCharacterDTO> enemies)
    {
        var enemyCharacters = new Dictionary<Guid, BattleCharacter>();
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
            enemyCharacters.Add(enemy.Id, enemyBattleCharacter);

            horizontalOffset += 5;
        }

        return enemyCharacters;
    }

    //TODO: Auto space spawns based on the number of characters instead of predefining positions
    private GameObject SpawnCharacter(int horizontalOffset, int verticalOffset, bool isEnemy)
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
