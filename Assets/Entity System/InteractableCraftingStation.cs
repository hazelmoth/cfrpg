using UnityEngine;

public class InteractableCraftingStation : MonoBehaviour, InteractableObject, ICraftingStation
{
    [SerializeField] private string workstationName;
    [SerializeField] private CraftingEnvironment environment;
    string ICraftingStation.WorkstationName => workstationName;

    CraftingEnvironment ICraftingStation.Environment => environment;

    void InteractableObject.OnInteract()
    {

    }
}
