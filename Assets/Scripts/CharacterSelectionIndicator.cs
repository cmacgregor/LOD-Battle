using UnityEngine;

public class CharacterSelectionIndicator : MonoBehaviour
{
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
        switch (HealthTierCalculator.GetTier(currentHealth, maxHealth))
        {
            case HealthTier.High:
                SetColorToHighHealthColor();
                break;
            case HealthTier.Mid:
                SetColorToMidHealthColor();
                break;
            case HealthTier.Low:
                SetColorToLowHealthColor();
                break;
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
