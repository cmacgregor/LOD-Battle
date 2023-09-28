using System;
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
    public BattleCamera _battleCamera;

    public BattleState _battleState = BattleState.Waiting;
    public BattleResult _battleResult = BattleResult.None;

    public BattleCharacterSpawner _battleCharacterSpawner;

    public int _escapeSuccessChance;
    public bool isBossEncounter;

    public Guid _actingCharacterId;
    private Dictionary<Guid, int> TurnPriorities = new Dictionary<Guid, int>();
    private const int TURN_THRESHOLD = 217;

    private IList<Guid> PlayerPartyIds = new List<Guid>();
    private IList<Guid> EnemyPartyIds = new List<Guid>();

    private Dictionary<Guid, PlayerMemberBattleCharacter> PlayerCharacters = new Dictionary<Guid, PlayerMemberBattleCharacter>();
    private Dictionary<Guid, BattleCharacter> EnemyCharacters = new Dictionary<Guid, BattleCharacter>();

    public Dictionary<Guid, CharacterStatusPanelController> PartyStatusPanels = new Dictionary<Guid, CharacterStatusPanelController>();


    void Start()
    {
        //these should be passed in
        var _playerParty = CreateParty();
        var _enemies = CreateEnemyParty();
        isBossEncounter = false;

        SetupCharacters(_playerParty, _enemies);

        HidePlayerUI();

        PlayBattleIntro();

        UpdateCombatantTurnPriorites(_actingCharacterId);

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

    private void SetupCharacters(IEnumerable<BattleCharacterDTO> playerParty, IEnumerable<BattleCharacterDTO> enemies)
    {
        CreatePlayerParty(playerParty);
        CreateEnemies(enemies);
    }

    private void CreatePlayerParty(IEnumerable<BattleCharacterDTO> playerParty)
    {
        //TODO: find a better way of handling getting these items mapped to party members
        var playerPartyStatusPanels = new List<CharacterStatusPanelController>
        {
            PlayerLeftCharacterStatusPanel.GetComponent<CharacterStatusPanelController>(),
            PlayerCenterCharacterStatusPanel.GetComponent<CharacterStatusPanelController>(),
            PlayerRightCharacterStatusPanel.GetComponent<CharacterStatusPanelController>()
        };

        var characterNumber = 0;
        var verticalOffset = -5;
        var horizontalOffset = -5;
        foreach (var partyMember in playerParty)
        {
            playerPartyStatusPanels[characterNumber].Setup(partyMember.Name, (PlayerPartyStats)partyMember.Stats);

            var partyMemberGameObject = _battleCharacterSpawner.SpawnCharacter(horizontalOffset, verticalOffset, false);
            var partyMemberBattleCharacter = partyMemberGameObject.GetComponent<PlayerMemberBattleCharacter>();
            partyMemberBattleCharacter.SetupCharacter(partyMember.Id, partyMember.Name, partyMember.Stats, partyMember.Element, partyMember.ModelName);
            PlayerCharacters.Add(partyMember.Id, partyMemberBattleCharacter);

            PlayerPartyIds.Add(partyMember.Id);
            PartyStatusPanels.Add(partyMember.Id, playerPartyStatusPanels[characterNumber]);
            TurnPriorities.Add(partyMember.Id, 0);

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
            var enemyGameObject = _battleCharacterSpawner.SpawnCharacter(horizontalOffset, verticalOffset, true);
            var enemyBattleCharacter = enemyGameObject.GetComponent<BattleCharacter>();
            enemyBattleCharacter.SetupCharacter(enemy.Id, enemy.Name, enemy.Stats, enemy.Element, enemy.ModelName);

            EnemyCharacters.Add(enemy.Id, enemyBattleCharacter);
            EnemyPartyIds.Add(enemy.Id);
            TurnPriorities.Add(enemy.Id, 0);

            horizontalOffset += 5;
        }
    }

    private void PlayBattleIntro()
    {
        if(isBossEncounter)
        {
            StartCoroutine(_battleCamera.BossIntroPan());
        }
        StartCoroutine(_battleCamera.BattleIntroPan());
    }

    private BattleState DetermineNextTurn()
    {
        _actingCharacterId = GetHighestTPCharacterId();
        UpdateCombatantTurnPriorites(_actingCharacterId);

        var returnState = BattleState.EnemyTurn;
        if (PlayerPartyIds.Contains(_actingCharacterId))
        {
            returnState = BattleState.PlayerTurn;
        }

        return returnState;
    }

    private Guid GetHighestTPCharacterId()
    {
        return TurnPriorities.Aggregate((tp1, tp2) => tp1.Value > tp2.Value ? tp1 : tp2).Key;
    }

    private void UpdateCombatantTurnPriorites(Guid turnCharacterId)
    {
        foreach (var id in TurnPriorities.Keys.ToList())
        {
            if (id == turnCharacterId)
            {
                TurnPriorities[id] = Math.Max(0, TurnPriorities[id] - TURN_THRESHOLD);
            }
            else
            {
                var characterSpeed = PlayerCharacters.Keys.Contains(id) ? PlayerCharacters[id].Speed : EnemyCharacters[id].Speed;
                var lowRoll = (int)(characterSpeed - (characterSpeed * 0.1));
                var highRoll = (int)(characterSpeed + (characterSpeed * 0.1));
                TurnPriorities[id] += (int)Random.Range(lowRoll, highRoll);
            }
        }
    }

    private void TakeEnemyTurn(Guid actingCharacterId)
    {
        //TODO: Create and apply enemy UI
        Debug.Log($"Enemey Turn Taken with {EnemyCharacters[actingCharacterId].Name}");
    }

    private void PlayerTurn(Guid actingCharacterId)
    {
        Debug.Log($"Player turn with {PlayerCharacters[actingCharacterId].Name}");

        PlayerCharacters[actingCharacterId].ApplyPreTurnStatusEffects();
        
        if(PlayerCharacters[actingCharacterId].StatusAilment == StatusAilments.Confusion)
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

        PlayerCharacters[actingCharacterId].ApplyPreTurnStatusEffects();

        ShowPlayerUI();
    }

    private void SetupPlayerActionsForPartyMember(Guid actingCharacterId)
    {
        //TODO: Set dragoon tranform icon color

        //TODO: Show special if all players spirit bars are maxed

        if (PlayerCharacters[actingCharacterId].StatusAilment == StatusAilments.ArmBlock)
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
        //TODO: Perform targe selection
        //TODO: check for confusion to change target list
        var targets = PlayerPartyIds.Contains(_actingCharacterId) 
            ? PlayerPartyIds : EnemyPartyIds;
        var selectedTarget = StartCoroutine(SingleTargetSelect(targets));

        _battleState = BattleState.NewTurn;
    }

    private IEnumerator<Guid> SingleTargetSelect(IList<Guid> possibleTargets)
    {
        //TODO: Pan camera to current target
        yield return possibleTargets.First();
    }

    public void OnPlayerDefendButton()
    {
        Debug.Log($"Defending with {PlayerCharacters[_actingCharacterId].Name}");

        PlayerCharacters[_actingCharacterId].Defend();
        PartyStatusPanels[_actingCharacterId].CurrentHealth = PlayerCharacters[_actingCharacterId].CurrentHealth;

        _battleState = BattleState.NewTurn;
    }

    public void OnPlayerItemButton()
    {
        Debug.Log($"Using Item with {PlayerCharacters[_actingCharacterId].Name}");

        //TODO: Create item selection, target selection, and application workflow

        _battleState = BattleState.NewTurn;
    }

    public void OnPlayerEscapeButton()
    {
        HidePlayerUI();

        Debug.Log($"Escaping with {PlayerCharacters[_actingCharacterId].Name}");

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

    private IEnumerable<BattleCharacterDTO> CreateParty()
    {
        return new List<BattleCharacterDTO>
        {
            new BattleCharacterDTO() {
                Id = Guid.NewGuid(),
                Name = "Dart",
                ModelName = "dart_model",
                Stats= new PlayerPartyStats(){
                    CurrentHealth = 1,
                    MaxHealth = 30,
                    Attack = 2,
                    Defense = 4,
                    MagicAttack = 3,
                    MagicDefense = 4,
                    Speed = 50,
                    CurrentMagic = 0,
                    MaxMagic = 10,
                    CurrentSpirit = 0,
                    CurrentSpiritBars = 0,
                    MaxSpiritBars = 1,
                },               
                Element = ElementAlignment.Fire,
            },
            new BattleCharacterDTO() {
                Id = Guid.NewGuid(),
                Name = "Rose",
                ModelName = "rose_model",
                Stats = new PlayerPartyStats(){
                    CurrentHealth = 21,
                    MaxHealth = 21,
                    Attack = 3,
                    Defense = 5,
                    MagicAttack = 6,
                    MagicDefense = 5,
                    Speed = 55,
                    CurrentMagic = 10,
                    MaxMagic = 10,
                    CurrentSpirit = 0,
                    CurrentSpiritBars = 1,
                    MaxSpiritBars = 1,
                },
                Element = ElementAlignment.Darkness,
            },
            new BattleCharacterDTO()
            {
                Id = Guid.NewGuid(),
                Name = "Meru",
                ModelName = "meru_model",
                Stats = new PlayerPartyStats(){
                    CurrentHealth = 18,
                    MaxHealth = 18,
                    Attack = 2,
                    Defense = 2,
                    MagicAttack = 3,
                    MagicDefense = 4,
                    Speed = 70,
                    CurrentMagic = 10,
                    MaxMagic = 10,
                    CurrentSpirit = 0,
                    CurrentSpiritBars = 1,
                    MaxSpiritBars = 1,
                },
                Element = ElementAlignment.Water,

            }
        };
    }

    private IEnumerable<BattleCharacterDTO> CreateEnemyParty()
    {
        return new List<BattleCharacterDTO>
        {
            new BattleCharacterDTO() {
                Id = Guid.NewGuid(),
                Name = "Assassin Cock",
                ModelName = "assassincock_model",
                Stats = new CharacterBattleStats()
                {
                    CurrentHealth = 3,
                    MaxHealth = 3,
                    Attack = 2,
                    Defense = 100,
                    MagicAttack = 3,
                    MagicDefense = 120,
                    Speed = 50,
                },
                Element = ElementAlignment.Wind,
            },
            new BattleCharacterDTO() {
                Id = Guid.NewGuid(),
                Name = "Berserk Mouse",
                ModelName = "berserkmouse_model",
                Stats = new CharacterBattleStats()
                {
                    CurrentHealth = 2,
                    MaxHealth = 2,
                    Attack = 2,
                    Defense = 80,
                    MagicAttack = 2,
                    MagicDefense = 120,
                    Speed = 50,
                },
                Element = ElementAlignment.Darkness

            },
            new BattleCharacterDTO() {
                Id = Guid.NewGuid(),
                Name = "Trent",
                ModelName = "trent_model",
                Stats = new CharacterBattleStats()
                {
                    CurrentHealth = 5,
                    MaxHealth = 5,
                    Attack = 3,
                    Defense = 160,
                    MagicAttack = 3,
                    MagicDefense = 120,
                    Speed = 30,
                },
                Element = ElementAlignment.Earth,
            }
        };
    }

}
