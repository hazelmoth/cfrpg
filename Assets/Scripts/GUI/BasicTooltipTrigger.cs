using UnityEngine;

public class BasicTooltipTrigger : MonoBehaviour, ITooltipTrigger
{
    [SerializeField] private string text;
    
    public bool DoShowTooltip => true;

    public string GetText()
    {
        return text;
    }
}
