using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStatusPanelController : MonoBehaviour
{
    public GameObject PortraitHighlighterObject;
    public GameObject PortraitObject;
    public GameObject NameObject;
    public GameObject CurrentHealthObject;
    public GameObject MaxHealthObject;
    public GameObject CurrentMagicObject;
    public GameObject MaxMagicObject;
    public GameObject CurrentSpiritBarsObject;
    public GameObject SpiritBarObject;
    public GameObject SpiritBarMaxValueHighlighterObject;

    public int MaxHP;
    public int MaxMP;
    public int MaxSpirit;

    private Image image_CharacterPortraitHighlighter;
    private Image image_CharacterPortrait;
    private TextMeshProUGUI textmeshpro_Name;
    private TextMeshProUGUI textmeshpro_CurrentHealth;
    private TextMeshProUGUI textmeshpro_MaxHealth;
    private TextMeshProUGUI textmeshpro_CurrentMagic;
    private TextMeshProUGUI textmeshpro_MaxMagic;
    private TextMeshProUGUI textmeshpro_CurrentSpiritBars;
    private Slider slider_SpiritBar;
    private Image image_SpiritSliderBackground;
    private Image image_SpiritSliderFill;
    private Image image_SpiritBarMaxHighlighter;
    
    void Start()
    {
        image_CharacterPortraitHighlighter = PortraitHighlighterObject.GetComponent<Image>();
        image_CharacterPortrait = PortraitObject.GetComponent<Image>();

        textmeshpro_Name = NameObject.GetComponent<TextMeshProUGUI>();

        textmeshpro_CurrentHealth = CurrentHealthObject.GetComponent<TextMeshProUGUI>();
        textmeshpro_MaxHealth = MaxHealthObject.GetComponent<TextMeshProUGUI>();

        textmeshpro_CurrentMagic = CurrentMagicObject.GetComponent<TextMeshProUGUI>();
        textmeshpro_MaxMagic = MaxMagicObject.GetComponent<TextMeshProUGUI>();

        textmeshpro_CurrentSpiritBars = CurrentSpiritBarsObject.GetComponent<TextMeshProUGUI>();

        slider_SpiritBar = SpiritBarObject.GetComponentInChildren<Slider>();

        var sliderImages = slider_SpiritBar.GetComponentsInChildren<Image>();
        image_SpiritSliderBackground = sliderImages.FirstOrDefault(i => i.name == "Background");
        image_SpiritSliderFill = sliderImages.FirstOrDefault(i => i.name == "Fill");

        image_SpiritBarMaxHighlighter = SpiritBarMaxValueHighlighterObject.GetComponent<Image>();
    }

    public void Setup(PlayerMemberBattleCharacter character)
    {
        image_CharacterPortrait.sprite = Resources.Load<Sprite>($"BattleSprites\\{character.Name}");
        textmeshpro_Name.SetText(character.Name);

        textmeshpro_CurrentHealth.SetText(character.CurrentHealth.ToString());
        textmeshpro_MaxHealth.SetText(character.MaxHealth.ToString());

        textmeshpro_CurrentMagic.SetText(character.CurrentMagic.ToString());
        textmeshpro_MaxMagic.SetText(character.MaxMagic.ToString());

        MaxHP = character.MaxHealth;
        MaxMP = character.MaxMagic;
        MaxSpirit = character.MaxSpirit;

        SetSpiritBarState(character.CurrentSpirit);

        //register events
        character.onCharacterHealthChange += UpdateCurrentHealth;
        character.onMagicUsed += UpdateCurrentMP;
        character.onSpiritUsed += UpdateCurrentSpirit;
    }

    public void ShowTurnCharacterIndicator()
    {
        image_CharacterPortraitHighlighter.enabled = true;
    }

    public void HidTurnCharacterIndicator()
    {
        image_CharacterPortraitHighlighter.enabled = false;
    }

    public void UpdateCurrentHealth(int newHealthValue)
    {
        var clampedHealthValue = StatClamp.Clamp(newHealthValue, MaxHP);
        if (clampedHealthValue != newHealthValue)
        {
            Debug.LogWarning($"Health value {newHealthValue} out of range, clamped to {clampedHealthValue}");
        }

        textmeshpro_CurrentHealth.SetText(clampedHealthValue.ToString());
        //set health text color if 0?
    }

    public void UpdateCurrentSpirit(int newSpiritValue)
    {
        var clampedSpiritValue = StatClamp.Clamp(newSpiritValue, MaxSpirit);
        if (clampedSpiritValue != newSpiritValue)
        {
            Debug.LogWarning($"Spirit value {newSpiritValue} out of range, clamped to {clampedSpiritValue}");
        }

        SetSpiritBarState(clampedSpiritValue);
    }

    public void UpdateCurrentMP(int newMPValue)
    {
        var clampedMPValue = StatClamp.Clamp(newMPValue, MaxMP);
        if (clampedMPValue != newMPValue)
        {
            Debug.LogWarning($"MP value {newMPValue} out of range, clamped to {clampedMPValue}");
        }

        textmeshpro_CurrentMagic.SetText(clampedMPValue.ToString());
    }

    private void SetSpiritBarState(int currentSpirit)
    {
        int currentSpiritBarCount = SpiritBarMath.GetBarCount(currentSpirit);
        var currentSpiritBarLevel = SpiritBarMath.GetBarLevel(currentSpirit);

        slider_SpiritBar.value = currentSpiritBarLevel;

        image_SpiritSliderFill.color = SpiritBarMath.GetBarColor(currentSpiritBarCount);
        image_SpiritSliderBackground.color = SpiritBarMath.GetBarColor(currentSpiritBarCount - 1);

        if (currentSpiritBarLevel == 0)
        {
            image_SpiritSliderFill.enabled = false;
        }
        else if (!image_SpiritSliderFill.enabled)
        {
            image_SpiritSliderFill.enabled = true;
        }

        if (currentSpirit == MaxSpirit)
        {
            image_SpiritBarMaxHighlighter.enabled = true;
        }
        else if (image_SpiritBarMaxHighlighter.enabled)
        {
            image_SpiritBarMaxHighlighter.enabled = false;
        }

        textmeshpro_CurrentSpiritBars.SetText(currentSpiritBarCount.ToString());
    }
}
