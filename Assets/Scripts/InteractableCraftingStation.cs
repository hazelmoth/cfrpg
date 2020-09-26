using UnityEngine;

public class InteractableCraftingStation : MonoBehaviour, IInteractableObject, ICraftingStation
{
    [SerializeField] private string workstationName;
    [SerializeField] private CraftingEnvironment environment;
    string ICraftingStation.WorkstationName => workstationName;

    CraftingEnvironment ICraftingStation.Environment => environment;

    void IInteractableObject.OnInteract()
    {

    }
}
