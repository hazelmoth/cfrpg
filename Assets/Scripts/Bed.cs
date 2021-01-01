using UnityEngine;

public class Bed : MonoBehaviour, IInteractableObject, IBed
{
    [SerializeField] private Transform sleepPosition;

    public float SpriteRotation => 0f;

    public Vector2 SleepPositionWorldCoords
    {
        get
        {
            if (sleepPosition == null) Debug.LogWarning("Bed doesn't have sleep position set!", this);
            return sleepPosition ? sleepPosition.position : transform.position;
        }
    }


    void IInteractableObject.OnInteract()
    {
        // This needs to be interactable for the player to use it,
        // but doesn't need to do anything.
    }
}
