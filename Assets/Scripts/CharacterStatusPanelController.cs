using System;
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
        if(newHealthValue < 0)
        {
            Console.WriteLine("Error: Cannot set health to less than 0");
            newHealthValue = 0;
        }
        else if(newHealthValue > MaxHP) {
            Console.WriteLine("Error: Cannot set health to greater than characters max health");
            newHealthValue = MaxHP;

        }

        textmeshpro_CurrentHealth.SetText(newHealthValue.ToString());
        //set health text color if 0? 
    }

    public void UpdateCurrentSpirit(int newSpiritValue)
    {
        if (newSpiritValue < 0)
        {
            Console.WriteLine("Error: Cannot set spirit to less than 0");
            newSpiritValue = 0;
        }

        else if (newSpiritValue > MaxSpirit)
        {
            Console.WriteLine("Error: Cannot set spirit to greater than characters max spirit");
            newSpiritValue = MaxSpirit;
        }

        SetSpiritBarState(newSpiritValue);
    }

    public void UpdateCurrentMP(int newMPValue)
    {
        if (newMPValue < 0)
        {
            Console.WriteLine("Error: Cannot set MP to less than 0");
            newMPValue = 0;
        }

        else if (newMPValue > MaxSpirit)
        {
            Console.WriteLine("Error: Cannot set MP to greater than characters max MP");
            newMPValue = MaxMP;
        }

        textmeshpro_CurrentMagic.SetText(newMPValue.ToString());
    }

    private void SetSpiritBarState(int currentSpirit)
    {
        int currentSpiritBarCount = currentSpirit / 100;
        var currentSpiritBarLevel = currentSpirit % 100;

        slider_SpiritBar.value = currentSpirit;

        image_SpiritSliderFill.color = GetSpiritLevelColor(currentSpiritBarCount);
        image_SpiritSliderBackground.color = GetSpiritLevelColor(currentSpiritBarCount- 1);

        if (currentSpiritBarLevel == 0)
        {
            image_SpiritSliderFill.enabled = false;
        }
        else if (!slider_SpiritBar.enabled)
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

    private Color GetSpiritLevelColor(int spiritLevel)
    {
        var returnColor = Color.clear;
        switch(spiritLevel){
            case 1:
                returnColor = Color.blue;
                break;
            case 2: 
                returnColor = Color.green;
                break;
            case 3:
                returnColor = Color.yellow; 
                break;
            case 4:
                returnColor = Color.HSVToRGB(255, 165, 0); //orange
                break;
            case 5:
                returnColor = Color.red;
                break;
        }

        return returnColor; 
    }
}
