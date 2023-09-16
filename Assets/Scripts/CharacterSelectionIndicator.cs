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
    
    public void SetColorToHighHealthColor()
    {
        this.gameObject.GetComponent<Renderer>().material = Resources.Load<Material>("Materials\\HighHealth");
    }

    public void SetColorToMidHealthColor() 
    {
        this.gameObject.GetComponent<Renderer>().material = Resources.Load<Material>("Materials\\MidHealth");
    }

    public void SetColorToLowHealthColor() 
    {
        this.gameObject.GetComponent<Renderer>().material = Resources.Load<Material>("Materials\\LowHealth");
    }
}
