using System;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
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

    public bool IsCharacterTurn;
    public Sprite PortraitSprite;
    public string CharacterName;
    public int CurrentHealth;
    public int MaxHealth;
    public int CurrentMagic;
    public int MaxMagic;
    public int CurrentSpiritBarProgress;
    public int SpiritBarCount;
    public int MaxSpiritBars;

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
    
    // Start is called before the first frame update
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

    // Update is called once per frame
    void Update()
    {
        image_CharacterPortraitHighlighter.enabled = IsCharacterTurn;
        image_CharacterPortrait.sprite = PortraitSprite;

        textmeshpro_Name.SetText(CharacterName);

        textmeshpro_CurrentHealth.SetText(CurrentHealth.ToString());
        textmeshpro_MaxHealth.SetText(MaxHealth.ToString());

        textmeshpro_CurrentMagic.SetText(CurrentMagic.ToString());
        textmeshpro_MaxMagic.SetText(MaxMagic.ToString());

        textmeshpro_CurrentSpiritBars.SetText(SpiritBarCount.ToString());
    }

    public void Setup(string name, PartyMemberStats partyMemberStats)
    {
        PortraitSprite = Resources.Load<Sprite>($"BattleSprites\\{name}");
        this.CharacterName = name;
        CurrentHealth = partyMemberStats.CurrentHealth;
        MaxHealth = partyMemberStats.MaxHealth;
        CurrentMagic = partyMemberStats.CurrentMagic;
        MaxMagic = partyMemberStats.MaxMagic;
        CurrentSpiritBarProgress = partyMemberStats.currentSpirit;
        SpiritBarCount = partyMemberStats.currentSpiritBars;
        MaxSpiritBars = partyMemberStats.maxSpiritBars;

        SetSpiritBarState(CurrentSpiritBarProgress, SpiritBarCount);
    }

    public void AddtoSpiritBar(int valueToAdd)
    {
        var newSpiritBarProgressValue = valueToAdd + CurrentSpiritBarProgress;
        if(newSpiritBarProgressValue >= 100 && SpiritBarCount < MaxSpiritBars)
        {
            SpiritBarCount++;
            CurrentSpiritBarProgress = newSpiritBarProgressValue - 100;
        }
        SetSpiritBarState(CurrentSpiritBarProgress, SpiritBarCount);
    }

    public void DecrementSpiritBar()
    {
        CurrentSpiritBarProgress = 0;
        if(SpiritBarCount > 0)
        {
            SpiritBarCount--;
        }
        SetSpiritBarState(CurrentSpiritBarProgress, SpiritBarCount);
    }
    private void SetSpiritBarState(int currentSpriritBarProgress, int currentSpiritBarLevel)
    {
        if (currentSpriritBarProgress > 100)
        {
            Console.WriteLine("Error: Cannot set spirit bar progress to greater than 100");
            return;
        }
        if (currentSpiritBarLevel > MaxSpiritBars)
        {
            Console.WriteLine("Error: Cannot set spirit bar level to above max spirit bars");
            return;
        }

        slider_SpiritBar.value = currentSpriritBarProgress;

        image_SpiritSliderFill.color = GetSpiritLevelColor(currentSpiritBarLevel);
        image_SpiritSliderBackground.color = GetSpiritLevelColor(currentSpiritBarLevel - 1);

        if (currentSpiritBarLevel == 100 || currentSpiritBarLevel == 0)
        {
            image_SpiritSliderFill.enabled = false;
        }
        else if (!slider_SpiritBar.enabled)
        {
            image_SpiritSliderFill.enabled = true;
        }

        if (currentSpiritBarLevel == MaxSpiritBars)
        {
            image_SpiritBarMaxHighlighter.enabled = true;
        }
        else if (image_SpiritBarMaxHighlighter.enabled)
        {
            image_SpiritBarMaxHighlighter.enabled = false;
        }
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
