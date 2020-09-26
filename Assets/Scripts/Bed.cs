using UnityEngine;

public class Bed : MonoBehaviour, IInteractableObject, IBed
{
    void IInteractableObject.OnInteract()
    {
        // Beds don't need to do anything
    }
}
