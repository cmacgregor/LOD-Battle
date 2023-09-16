using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public enum BattleState { 
    Waiting,
    NewTurn,
    PlayerTurn, 
    EnemyTurn,
    Finished,
}

public enum BattleResult
{
    None,
    Won,
    Lost,
    Fled,
}

public enum PlayerCommands {
    Attack,
    Defend,
    Item,
    Escape,
    DragoonTransform,
    DragoonAttack,
    DragoonMagic,
    Special
}

//TODO: Space player actions objects on a grid so they can be created and added to dynamically
public class BattleSystem : MonoBehaviour
{
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

    public BattleState _battleState = BattleState.Waiting;
    public BattleResult _battleResult = BattleResult.None;

    public Guid _actingCharacterId;

    public BattleSystemDataContext _battleSystemDataContext;
    public BattleCharacterSpawner _battleCharacterSpawner;

    public int _escapeSuccessChance; 

    void Start()
    {
        //these should be passed in
        var _playerParty = CreateParty();
        var _enemies = CreateEnemyParty();

        _battleCharacterSpawner = GetComponent<BattleCharacterSpawner>();
        _battleSystemDataContext = CreateBattleSystemDataContext(_playerParty, _enemies);

        HidePlayerUI();

        PlayBattleIntro();

        _battleSystemDataContext.UpdateCombatantTurnPriorites(_actingCharacterId);

        _battleState = BattleState.NewTurn;
    }

    void Update()
    {
        if (_battleResult != BattleResult.None && _battleState != BattleState.Finished)
        {
            _battleState = BattleState.Finished;
            EndBattle(_battleResult);
        }

        if (_battleState == BattleState.Waiting || _battleState == BattleState.Finished)
        {
            return;
        }
        else if (_battleState == BattleState.PlayerTurn)
        {
            _battleState = BattleState.Waiting;
            PlayerTurn(_actingCharacterId);
        }
        else if (_battleState == BattleState.EnemyTurn)
        {
            TakeEnemyTurn(_actingCharacterId);
            _battleState = BattleState.NewTurn;
        }
        else
        {
            HidePlayerUI();
            _battleState = DetermineNextTurn();
        }
    }

    private BattleSystemDataContext CreateBattleSystemDataContext(IEnumerable<PartyMemberBattleCharacter> playerParty, IEnumerable<EnemyBattleCharacter> enemies)
    {
        var battleSystemDataContext = new BattleSystemDataContext();

        //TODO: find a better way of handling getting these items mapped to party members
        var playerPartyStatusPanels = new List<CharacterStatusPanelController>
        {
            PlayerLeftCharacterStatusPanel.GetComponent<CharacterStatusPanelController>(),
            PlayerCenterCharacterStatusPanel.GetComponent<CharacterStatusPanelController>(),
            PlayerRightCharacterStatusPanel.GetComponent<CharacterStatusPanelController>()
        };

        var playerPartyObjects = _battleCharacterSpawner.SpawnCharacterLine(playerParty.Select(e => new Tuple<Guid, string>(e.Id, e.ModelName)).ToList(), false);
        int characterNumber = 0;

        foreach (var partyMember in playerParty)
        {
            playerPartyStatusPanels[characterNumber].Setup(partyMember.Name, partyMember.CombatStats.CurrentHealth, partyMember.CombatStats.MaxHealth, partyMember.PartyStats);

            battleSystemDataContext.PlayerPartyIds.Add(partyMember.Id);
            battleSystemDataContext.PartyStatusPanels.Add(partyMember.Id, playerPartyStatusPanels[characterNumber]);
            battleSystemDataContext.TurnPriorities.Add(partyMember.Id, 0);
            battleSystemDataContext.BattleCharacters.Add(partyMember.Id, partyMember);
            battleSystemDataContext.CharacterGameObjects.Add(partyMember.Id, playerPartyObjects[partyMember.Id]);
            battleSystemDataContext.CharacterSelectionIndicators.Add(partyMember.Id, playerPartyObjects[partyMember.Id].GetComponentInChildren<CharacterSelectionIndicator>());
            battleSystemDataContext.SetCharacterHealthIndicatorColor(partyMember.Id);

            characterNumber++;
        }

        var enemyObjects = _battleCharacterSpawner.SpawnCharacterLine(enemies.Select(e => new Tuple<Guid, string>(e.Id, e.ModelName)).ToList(), true);

        foreach (var enemy in enemies)
        {
            battleSystemDataContext.EnemyPartyIds.Add(enemy.Id);
            battleSystemDataContext.TurnPriorities.Add(enemy.Id, 0);
            battleSystemDataContext.BattleCharacters.Add(enemy.Id, enemy);
            battleSystemDataContext.CharacterGameObjects.Add(enemy.Id, enemyObjects[enemy.Id]);
            battleSystemDataContext.CharacterSelectionIndicators.Add(enemy.Id, enemyObjects[enemy.Id].GetComponentInChildren<CharacterSelectionIndicator>());
            battleSystemDataContext.SetCharacterHealthIndicatorColor(enemy.Id);
        }

        return battleSystemDataContext;
    }

    private void PlayBattleIntro()
    {
        //TODO: if this is a boss battle play boss battle intro. Otherwise pick random chamera pan
        StartCoroutine(PanCamera());
    }

    private IEnumerator PanCamera()
    {
        //TODO: Pan camera around and then show UI
        //  pan camera around
        //  put script on main camera that pans it around based on enum passed in
        //  call that here based on the type of panning desired before the battle
        //  wait for the amount of time (frames?) the camera animation will take
        yield return new WaitForSeconds(2f);
    }

    private BattleState DetermineNextTurn()
    {
        _actingCharacterId = _battleSystemDataContext.GetHighestTPCharacterId();
        _battleSystemDataContext.UpdateCombatantTurnPriorites(_actingCharacterId);

        var returnState = BattleState.EnemyTurn;
        if (_battleSystemDataContext.IsPartyMember(_actingCharacterId))
        {
            returnState = BattleState.PlayerTurn;
        }

        return returnState;
    }

    private void TakeEnemyTurn(Guid actingCharacterId)
    {
        //TODO: Create and apply enemy UI
        Debug.Log($"Enemey Turn Taken with {_battleSystemDataContext.BattleCharacters[actingCharacterId].Name}");
    }

    private void PlayerTurn(Guid actingCharacterId)
    {
        Debug.Log($"Player turn with {_battleSystemDataContext.BattleCharacters[actingCharacterId].Name}");

        _battleSystemDataContext.BattleCharacters[actingCharacterId].ApplyPreTurnStatusEffects();
        
        if(_battleSystemDataContext.BattleCharacters[actingCharacterId].StatusAilment == StatusAilments.Confusion)
        {
            //TODO: Select a random action 
            //  Attack random target (allies included)
            //  Guard
            //  Attempt to flee
        }

        SetupUIForPlayerTurn(actingCharacterId);
    }

    private void SetupUIForPlayerTurn(Guid actingCharacterId)
    {
        //change any action elements based upon character selection or state
        SetupPlayerActionsForPartyMember(actingCharacterId);

        _battleSystemDataContext.BattleCharacters[actingCharacterId].ApplyPreTurnStatusEffects();

        ShowPlayerUI();
    }

    private void SetupPlayerActionsForPartyMember(Guid actingCharacterId)
    {
        //TODO: Set dragoon tranform icon color

        //TODO: Show special if all players spirit bars are maxed

        if (_battleSystemDataContext.BattleCharacters[actingCharacterId].StatusAilment == StatusAilments.ArmBlock)
        {
            //TODO: Apply ArmBlocking status
        }

    }

    private void ShowPlayerUI()
    {
        PlayerPartyCanvas.SetActive(true);
        PlayerActionCanvas.SetActive(true);
    }

    private void HidePlayerUI()
    {
        PlayerActionCanvas.SetActive(false);
        PlayerPartyCanvas.SetActive(false);
    }

    private void EndBattle(BattleResult result)
    {
        Debug.Log(result.ToString());

        if (result == BattleResult.Lost)
        {
            //TODO: Handle scripted losses
            //TODO: Losing should play the loss sound and show the game over screen
        }
        else
        {
            if (result == BattleResult.Won)
            {
                //TODO: Play victory animations 
                //TODO: Do victory camera pan
            }

            if (result == BattleResult.Fled)
            {
                //TODO: play party retreat animation
                //TODO: leave battle screen

                //Only reward for killed enemies? 
            }

            //TODO: Go to rewards screen
        }
    }

    public void OnPlayerAttackButton()
    {
        _battleSystemDataContext.CharacterAttacks(_actingCharacterId);
        _battleState = BattleState.NewTurn;
    }

    public void OnPlayerDefendButton()
    {
        _battleSystemDataContext.CharacterDefends(_actingCharacterId);
        _battleState = BattleState.NewTurn;
    }

    public void OnPlayerItemButton()
    {
        Debug.Log($"Using Item with {_battleSystemDataContext.BattleCharacters[_actingCharacterId].Name}");

        //TODO: Create item selection, target selection, and application workflow

        _battleState = BattleState.NewTurn;
    }

    public void OnPlayerEscapeButton()
    {
        HidePlayerUI();

        Debug.Log($"Escaping with {_battleSystemDataContext.BattleCharacters[_actingCharacterId].Name}");

        if(CalculateEscapeSuccess(_escapeSuccessChance))
        {
            Debug.Log("Escaped successfully");

            _battleResult = BattleResult.Fled;
        }
        else
        {
            Debug.Log("Escaped failed");
            _battleState = BattleState.NewTurn;
        }
    }

    public bool CalculateEscapeSuccess(int areaEscapeChance)
    {
        //TODO: calculate escape change against areaEscapeChance
        //For now just wing it
        return Random.Range(0, 100) > 50;
    }

    private IEnumerable<PartyMemberBattleCharacter> CreateParty()
    {
        return new List<PartyMemberBattleCharacter>
        {
            new PartyMemberBattleCharacter() {
                Id = Guid.NewGuid(),
                Name = "Dart",
                ModelName = "dart_model",
                CombatStats = new CharacterStats()
                {
                    CurrentHealth = 1,
                    MaxHealth = 30,
                    Attack = 2,
                    Defense = 4,
                    MagicAttack = 3,
                    MagicDefense = 4,
                    Speed = 50,
                },
                PartyStats= new PlayerPartyStats(){
                    CurrentMagic = 0,
                    MaxMagic = 10,
                    currentSpirit = 0,
                    currentSpiritBars = 0,
                    maxSpiritBars = 1,
                }
            },
            new PartyMemberBattleCharacter() {
                Id = Guid.NewGuid(),
                Name = "Rose",
                ModelName = "rose_model",
                CombatStats = new CharacterStats()
                {
                    CurrentHealth = 21,
                    MaxHealth = 21,
                    Attack = 3,
                    Defense = 5,
                    MagicAttack = 6,
                    MagicDefense = 5,
                    Speed = 55,
                },
                PartyStats = new PlayerPartyStats(){
                    CurrentMagic = 10,
                    MaxMagic = 10,
                    currentSpirit = 0,
                    currentSpiritBars = 1,
                    maxSpiritBars = 1,
                }
            },
            new PartyMemberBattleCharacter()
            {
                Id = Guid.NewGuid(),
                Name = "Meru",
                ModelName = "meru_model",
                CombatStats = new CharacterStats()
                {
                    CurrentHealth = 18,
                    MaxHealth = 18,
                    Attack = 2,
                    Defense = 2,
                    MagicAttack = 3,
                    MagicDefense = 4,
                    Speed = 70,
                },
                PartyStats = new PlayerPartyStats(){
                    CurrentMagic = 10,
                    MaxMagic = 10,
                    currentSpirit = 0,
                    currentSpiritBars = 1,
                    maxSpiritBars = 1,
                },
            }
        };
    }

    private IEnumerable<EnemyBattleCharacter> CreateEnemyParty()
    {
        return new List<EnemyBattleCharacter>
        {
            new EnemyBattleCharacter() {
                Id = Guid.NewGuid(),
                Name = "Assassin Cock",
                ModelName = "assassincock_model",
                CombatStats = new CharacterStats()
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
            },
            new EnemyBattleCharacter() {
                Id = Guid.NewGuid(),
                Name = "Berserk Mouse",
                ModelName = "berserkmouse_model",
                CombatStats = new CharacterStats()
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
            },
            new EnemyBattleCharacter() {
                Id = Guid.NewGuid(),
                Name = "Trent",
                ModelName = "trent_model",
                CombatStats = new CharacterStats()
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
        };
    }

}
