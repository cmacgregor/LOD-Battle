using UnityEngine;

public class CharacterSelectionIndicator : MonoBehaviour
{
    public const decimal HIGH_HEALTH_PERCENTAGE = 2m / 3m;
    public const decimal LOW_HEALTH_PERCENTAGE = 1m / 3m;

    public void Show()
    {
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }


    public void SetIndicatorColor(decimal currentHealth, decimal maxHealth)
    {
        var healthPercentage = currentHealth / maxHealth;
        if (healthPercentage >= CharacterSelectionIndicator.HIGH_HEALTH_PERCENTAGE)
        {
            SetColorToHighHealthColor();
        }
        else if (healthPercentage < CharacterSelectionIndicator.HIGH_HEALTH_PERCENTAGE
            && healthPercentage > CharacterSelectionIndicator.LOW_HEALTH_PERCENTAGE)
        {
            SetColorToMidHealthColor();
        }
        else if (healthPercentage <= CharacterSelectionIndicator.LOW_HEALTH_PERCENTAGE)
        {
            SetColorToLowHealthColor();
        }
    }

    private void SetColorToHighHealthColor()
    {
        this.gameObject.GetComponent<Renderer>().material = Resources.Load<Material>("Materials\\HighHealth");
    }

    private void SetColorToMidHealthColor() 
    {
        this.gameObject.GetComponent<Renderer>().material = Resources.Load<Material>("Materials\\MidHealth");
    }

    private void SetColorToLowHealthColor() 
    {
        this.gameObject.GetComponent<Renderer>().material = Resources.Load<Material>("Materials\\LowHealth");
    }
}
