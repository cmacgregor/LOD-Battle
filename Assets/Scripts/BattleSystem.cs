using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public enum BattleState { 
    Waiting,
    NewTurn,
    PlayerTurn, 
    EnemyTurn, 
    Won, 
    Lost 
}

public enum PlayerCommands
{
    Attack,
    Defend,
    Item,
    Escape,
    DragoonTransform,
    DragoonAttack,
    DragoonMagic,
    Special
}

//TODO: Auto space spawns based on the number of characters instead of predefining positions
//TODO: Pan camera around and then show UI
//TODO: Space player actions objects on a grid so they can be created and added to dynamically
public class BattleSystem : MonoBehaviour
{
    public const int TURN_THRESHOLD = 217;

    public GameObject BattleCharacterPrefab;
    public GameObject PlayerPartyCanvas;
    public GameObject PlayerActionCanvas;
    public GameObject AttackCommand;
    public GameObject DefendCommand;
    public GameObject ItemCommand;
    public GameObject EscapeCommand;
    public GameObject DragoonTransformCommand;
    //public GameObject DragoonAttackCommand;
    //public GameObject DragoonMagicCommand;
    //public GameObject SpecialCommand;
    public GameObject PlayerCenterCharacterStatusPanel;
    public GameObject PlayerLeftCharacterStatusPanel;
    public GameObject PlayerRightCharacterStatusPanel;

    public BattleState battleState = BattleState.Waiting;
    public Guid actingCharacterId;

    public Dictionary<Guid, int> combatantTPValues = new Dictionary<Guid, int>();

    private IList<CharacterStatusPanelController> playerPartyStatusPanels;

    public Dictionary<Guid, PartyMemberBattleCharacter> playerParty;
    public Dictionary<Guid, EnemyBattleCharacter> enemies;

    private const int playerLineUpOffset = -5;
    private const int enemyLineUpOffset = 5;

    void Start()
    {
        CreateParty();
        CreateEnemyParty();

        SetupBattle();

        PlayBattleIntro();

        InitializeCombatantTurnPriority();

        battleState = BattleState.NewTurn;
    }

    void Update()
    {
        if(battleState == BattleState.Waiting)
        {
            return;
        }
        else if(battleState == BattleState.Won || battleState == BattleState.Lost)
        {
            EndBattle(battleState);
        }
        else if (battleState == BattleState.PlayerTurn)
        {
            PlayerTurn();
            battleState = BattleState.Waiting;
        }
        else if (battleState == BattleState.EnemyTurn)
        {
            TakeEnemyTurn();
            battleState = BattleState.NewTurn;
        }
        else
        {
            HideUIAfterPlayerAction();
            DetermineNextTurn();
        }
    }

    private void CreateParty()
    {
        var dartId = Guid.NewGuid();
        var roseId = Guid.NewGuid();
        var meruId = Guid.NewGuid();
        playerParty = new Dictionary<Guid, PartyMemberBattleCharacter>
        { { dartId, new PartyMemberBattleCharacter() {
                Id = dartId,
                Name = "Dart",
                Stats = new PartyMemberStats(){
                    CurrentHealth = 30,
                    MaxHealth = 30,
                    CurrentMagic = 0,
                    MaxMagic = 10,
                    currentSpirit = 0,
                    currentSpiritBars = 0,
                    maxSpiritBars = 1,
                    Attack = 2,
                    Defense = 4,
                    MagicAttack = 3,
                    MagicDefense = 4,
                    Speed = 50,
                }
            }
        },
        { roseId, new PartyMemberBattleCharacter() {
                Id = roseId,
                Name = "Rose",
                Stats = new PartyMemberStats(){
                    CurrentHealth = 21,
                    MaxHealth = 21,
                    CurrentMagic = 10,
                    MaxMagic = 10,
                    currentSpirit = 0,
                    currentSpiritBars = 1,
                    maxSpiritBars = 1,
                    Attack = 3,
                    Defense = 5,
                    MagicAttack = 6,
                    MagicDefense = 5,
                    Speed = 55,
                }
            }
        },
        {meruId, new PartyMemberBattleCharacter()
            {
                Id = Guid.NewGuid(),
                Name = "Meru",
                Stats = new PartyMemberStats(){
                    CurrentHealth = 18,
                    MaxHealth = 18,
                    CurrentMagic = 10,
                    MaxMagic = 10,
                    currentSpirit = 0,
                    currentSpiritBars = 1,
                    maxSpiritBars = 1,
                    Attack = 2,
                    Defense = 2,
                    MagicAttack = 3,
                    MagicDefense = 4,
                    Speed = 70,
                },
            }
        },
    };
    }

    private void CreateEnemyParty()
    {
        var enemy1Id = Guid.NewGuid();
        var enemy2Id = Guid.NewGuid();
        var enemy3Id = Guid.NewGuid();
        enemies = new Dictionary<Guid, EnemyBattleCharacter>()
        {
            { enemy1Id, new EnemyBattleCharacter() {
                    Id = enemy1Id,
                    Name = "Assassin Cock",
                    Stats = new CharacterStats()
                    {
                        CurrentHealth = 3,
                        MaxHealth = 3,
                        Attack = 2,
                        Defense = 100,
                        MagicAttack = 3,
                        MagicDefense = 120,
                        Speed = 50,
                        Element = ElementAlignment.Wind,
                    },
                    CanCounterAttack = true,
                    ExperienceReward = 5,
                    GoldReward = 6,
                    ItemDrops = new Dictionary<string, int>()
                    {
                        { "Healing Potion", 10 }
                    }
                }
            },
            { enemy2Id, new EnemyBattleCharacter() {
                    Id = enemy2Id,
                    Name = "Berserk Mouse",
                    Stats = new CharacterStats()
                    {
                        CurrentHealth = 2,
                        MaxHealth = 2,
                        Attack = 2,
                        Defense = 80,
                        MagicAttack = 2,
                        MagicDefense = 120,
                        Speed = 50,
                        Element = ElementAlignment.Darkness,
                    },
                    CanCounterAttack = true,
                    ExperienceReward = 3,
                    GoldReward = 3,
                    ItemDrops = new Dictionary<string, int>()
                    {
                        {"Healing Potion", 10 }
                    }
                }
            },
            { enemy3Id, new EnemyBattleCharacter() {
                    Id = enemy2Id,
                    Name = "Trent",
                    Stats = new CharacterStats()
                    {
                        CurrentHealth = 5,
                        MaxHealth = 5,
                        Attack = 3,
                        Defense = 160,
                        MagicAttack = 3,
                        MagicDefense = 120,
                        Speed = 30,
                        Element = ElementAlignment.Earth,
                    },
                    CanCounterAttack = true,
                    ExperienceReward = 4,
                    GoldReward = 9,
                    ItemDrops = new Dictionary<string, int>()
                    {
                        {"Pellet", 10 }
                    }
                }
             },
        };
    }
    
    private IEnumerator TakeEnemyTurn()
    {
        //take an enemy turn but pass for now
        Debug.Log("Enemey Turn Taken");

        yield return new WaitForSeconds(4f);
    }

    private void SetupBattle()
    {
        SetupUI();
        SpawnPlayerParty();
        SpawnEnemies();
    }
    
    private void SetupUI()
    {
        PlayerPartyCanvas.SetActive(false);
        PlayerActionCanvas.SetActive(false);

        playerPartyStatusPanels = new List<CharacterStatusPanelController>
        {
            PlayerLeftCharacterStatusPanel.GetComponent<CharacterStatusPanelController>(),
            PlayerCenterCharacterStatusPanel.GetComponent<CharacterStatusPanelController>(),
            PlayerRightCharacterStatusPanel.GetComponent<CharacterStatusPanelController>()
        };
        SetupPlayerPartyUI();
    }

    private void SetupPlayerPartyUI()
    {
        int characterNumber = 0;

        foreach (var character in playerParty.Values)
        {
            playerPartyStatusPanels[characterNumber].Setup(character.Name, character.Stats);
            characterNumber++;
        }
    }

    private void SpawnPlayerParty()
    {
        SpawnCharacter(new Vector3(0, 1, playerLineUpOffset), false);   // Center character
        SpawnCharacter(new Vector3(-5, 1, playerLineUpOffset), false);  // Left Character
        SpawnCharacter(new Vector3(5, 1, playerLineUpOffset), false);   // Right Character
    }

    private void SpawnEnemies()
    {
        //This is not effecient but it'll do until other items are handled
        List<Vector3> enemySpawnLocations = new List<Vector3>{
                    new Vector3(0, 1, enemyLineUpOffset),
                    new Vector3(-5, 1, enemyLineUpOffset),
                    new Vector3(5, 1, enemyLineUpOffset),
                };

        foreach(var location in enemySpawnLocations)
        {
            SpawnCharacter(location);
        }
    }

    private void SpawnCharacter(Vector3 position, bool isEnemy = true)
    {
        var spawn = Instantiate(BattleCharacterPrefab, position, Quaternion.identity);
        if(isEnemy)
        {
            spawn.GetComponent<Renderer>().material = Resources.Load<Material>("Materials\\Enemy");
        }
    }

    private void PlayBattleIntro()
    {
        StartCoroutine(PanCamera());
        PlayerPartyCanvas.SetActive(true);
    }

    private IEnumerator PanCamera()
    {
        //pan camera around
        //put script on main camera that pans it around based on enum passed in
        //call that here based on the type of panning desired before the battle
        //wait for the amount of time (frames?) the camera animation will take
        yield return new WaitForSeconds(2f);
    }

    private void InitializeCombatantTurnPriority()
    {
        foreach (var partyMemberIds in playerParty.Keys)
        {
            combatantTPValues.Add(partyMemberIds, Random.Range(0, TURN_THRESHOLD));
        }
        foreach(var enemyIds in enemies.Keys)
        {
            combatantTPValues.Add(enemyIds, Random.Range(0, TURN_THRESHOLD));
        }
    }

    private BattleState DetermineNextTurn()
    {
        var returnState = BattleState.EnemyTurn;

        actingCharacterId = GetHighestTPCharacterId();
        UpdateCombatantTurnPriorites(actingCharacterId);

        if (playerParty.ContainsKey(actingCharacterId))
        {
            returnState = BattleState.PlayerTurn;
        }

        return returnState;
    }

    private Guid GetHighestTPCharacterId()
    {
        var highestTPCharacterId = combatantTPValues.First().Key;
        var highestTP = 0;
        foreach (var character in combatantTPValues)
        {
            if (character.Value > highestTP)
            {
                highestTPCharacterId = character.Key;
                highestTP = character.Value;
            }
        }

        return highestTPCharacterId;
    }

    private void UpdateCombatantTurnPriorites(Guid turnCharacterId)
    {
        foreach (var id in combatantTPValues.Keys.ToList())
        {
            if(id == turnCharacterId)
            {
                combatantTPValues[id] = Mathf.Max(0, combatantTPValues[id] - TURN_THRESHOLD);
            }
            else
            {
                var characterSpeed = playerParty.Keys.Contains(turnCharacterId) ? playerParty[turnCharacterId].Stats.Speed : enemies[turnCharacterId].Stats.Speed; 

                var lowRoll = characterSpeed - (characterSpeed / 10);
                var highRoll = characterSpeed + (characterSpeed / 10);
                combatantTPValues[id] += Random.Range(lowRoll, highRoll);
            }
        }
    }

    private void PlayerTurn()
    {

        Debug.Log($"Player turn with {playerParty[actingCharacterId].Name}");
        SetupUIForPlayerTurn(actingCharacterId);
    }

    private void SetupUIForPlayerTurn(Guid actingCharacterKey)
    {
        //change any action elements based upon character selection or state

        PlayerActionCanvas.SetActive(true);
    }

    private void HideUIAfterPlayerAction()
    {
        PlayerActionCanvas.SetActive(false);
        PlayerLeftCharacterStatusPanel.SetActive(false);
    }

    private void EndBattle(BattleState result)
    {
        Debug.Log(result.ToString());

        //losing should result in Game Over screen
        //winning puts us on the post battle screen
    }

    public void OnPlayerAttackButton()
    {
        Debug.Log($"Attacking with {playerParty[actingCharacterId].Name}");
        HideUIAfterPlayerAction();
        battleState = BattleState.NewTurn;
    }

    public void OnPlayerDefendButton()
    {
        Debug.Log($"Defending with {playerParty[actingCharacterId].Name}");
        HideUIAfterPlayerAction();
        battleState = BattleState.NewTurn;
    }

    public void OnPlayerItemButton()
    {
        Debug.Log($"Using Item with {playerParty[actingCharacterId].Name}");
        HideUIAfterPlayerAction();
        battleState = BattleState.NewTurn;
    }

    public void OnPlayerEscapeButton()
    {
        Debug.Log($"Escaping with {playerParty[actingCharacterId].Name}");
        HideUIAfterPlayerAction();
        battleState = BattleState.NewTurn;
    }
}
