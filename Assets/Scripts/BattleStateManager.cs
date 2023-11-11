using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


//TODO: Space player actions objects on a grid so they can be created and added to dynamically
public class BattleStateManager : MonoBehaviour
{
    public GameObject PlayerPartyCanvas;
    public GameObject PlayerActionCanvas;

    public BattleCamera Camera;

    public BattleCharacterSpawner CharacterSpawner;
    public int EscapeRate;
    public bool IsBossEncounter;

    public Guid ActingCharacterId;
    public BattleBaseState CurrentState;
    public BattleIntroState IntroState = new BattleIntroState();
    public BattlePlayerTurnState PlayerTurnState = new BattlePlayerTurnState();
    public BattleEnemyTurnState EnemyTurnState = new BattleEnemyTurnState();
    public BattleFleeingState FleeingState = new BattleFleeingState();
    public BattleWonState WonState = new BattleWonState();
    public BattleLostState LostState = new BattleLostState();

    private TurnSelector _turnSelector;
    private Dictionary<Guid, PlayerMemberBattleCharacter> _playerCharacters = new Dictionary<Guid, PlayerMemberBattleCharacter>();
    private Dictionary<Guid, BattleCharacter> _enemyCharacters = new Dictionary<Guid, BattleCharacter>();

    void Start()
    {
        //these should be passed in
        var _playerParty = CreateParty();
        var _enemies = CreateEnemyParty();
        IsBossEncounter = false;
        EscapeRate = 25;

        _playerCharacters = CharacterSpawner.SetupPartyCharacters(_playerParty);
        _enemyCharacters = CharacterSpawner.SetupEnemyCharacters(_enemies);

        var characterSpeeds = _playerCharacters.ToDictionary(x => x.Key, x => x.Value.Speed);
        var enemeySpeeds = _enemyCharacters.ToDictionary(x => x.Key, x => x.Value.Speed);
        characterSpeeds.AddRange(enemeySpeeds);

        _turnSelector = new TurnSelector(characterSpeeds);

        CurrentState = IntroState;
        CurrentState.EnterState(this);
    }

    void Update()
    {
        CurrentState.UpdateState(this);
    }

    public void SwitchBattleState(BattleBaseState state)
    {
        CurrentState = state;
        state.EnterState(this);
    }

    public BattleCharacter GetActingCharacter()
    {
        if (IsPlayerPartyMember(ActingCharacterId))
        {
            return _playerCharacters[ActingCharacterId] as BattleCharacter;
        }

        return _enemyCharacters[ActingCharacterId];
    }

    public void DetermineNextTurn()
    {
        ActingCharacterId = _turnSelector.DetermineNextActingCharacter();
        if (IsPlayerPartyMember(ActingCharacterId))
        {
            SwitchBattleState(PlayerTurnState);
        }
        else
        {
            SwitchBattleState(EnemyTurnState);
        }
    }

    public int GetEscapeChance()
    {
        return EscapeRate;
    }

    public void Flee()
    {
        SwitchBattleState(FleeingState);
    }

    public IList<Guid> GetEnemyIds()
    {
        return _enemyCharacters.Keys.ToList();
    }

    public IDictionary<Guid, CharacterSelectionIndicator> GetEnemyIndicators()
    {
        return _enemyCharacters.ToDictionary( x => x.Key, x => x.Value.Indicator );
    }

    #region handle UI
    public void ShowPlayerUI()
    {
        PlayerPartyCanvas.SetActive(true);
        PlayerActionCanvas.SetActive(true);
    }

    public void HidePlayerUI()
    {
        PlayerActionCanvas.SetActive(false);
        PlayerPartyCanvas.SetActive(false);
    }

    public void SetupUIForPlayerTurn(Guid characterId)
    {
        SetupPlayerActionsForPartyMember(characterId);

        _playerCharacters[characterId].ApplyPreTurnStatusEffects();

        ShowPlayerUI();
    }

    public void SetupPlayerActionsForPartyMember(Guid characterId)
    {
        //TODO: Set dragoon tranform icon color

        //TODO: Show special if all players spirit bars are maxed

        if (_playerCharacters[characterId].StatusAilment == StatusAilments.ArmBlock)
        {
            //TODO: Apply ArmBlocking status
        }
    }
    #endregion

    private bool IsPlayerPartyMember(Guid id)
    {
        return _playerCharacters.Keys.Contains(id);
    }

    #region create character stats TODO: hand this to battle state manager on load
    private IEnumerable<BattleCharacterDTO> CreateParty()
    {
        return new List<BattleCharacterDTO>
        {
            new BattleCharacterDTO() {
                Id = Guid.NewGuid(),
                Name = "Dart",
                ModelName = "dart",
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
                    MaxSpirit = 100,
                },
                Element = ElementAlignment.Fire,
            },
            new BattleCharacterDTO() {
                Id = Guid.NewGuid(),
                Name = "Rose",
                ModelName = "rose",
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
                    CurrentSpirit = 35,
                    MaxSpirit = 200,
                },
                Element = ElementAlignment.Darkness,
            },
            new BattleCharacterDTO()
            {
                Id = Guid.NewGuid(),
                Name = "Meru",
                ModelName = "meru",
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
                    MaxSpirit = 100,
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
                ModelName = "assassin_cock",
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
                ModelName = "berserk_mouse",
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
                ModelName = "trent",
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
    #endregion
}
