/// A thing which non-player actors can stand behind to sell items.
public class ShopStation : NonPlayerWorkstation, IInteractable
{
    public void OnInteract()
    {
        if (Occupied) PlayerInteractionManager.InitiateTrade(CurrentOccupier.ActorId);
    }
}
