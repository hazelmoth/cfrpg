using System.Collections.Generic;

/// A thing which non-player actors can stand behind to sell items.
/// Also functions as a container for items which the store is selling.
public class ShopStation : NonPlayerWorkstation, IInteractable, ISaveable, IContainer
{
    private SaveableContainerData saveableContainer;

    private void Start()
    {
        saveableContainer = new SaveableContainerData(InitializeSlots());
    }

    public string Name => "Shop Counter";

    public int SlotCount => 24;

    public string ComponentId => nameof(ShopStation);

    public ItemStack Get(int slot)
    {
        return saveableContainer.Get(slot);
    }

    public void Set(int slot, ItemStack item)
    {
        saveableContainer.Set(slot, item);
    }

    public bool AcceptsItemType(string itemId, int slot)
    {
        return true;
    }

    public void OnInteract()
    {
        if (Occupied) PlayerInteractionManager.InitiateTrade(CurrentOccupier.ActorId);
    }

    public IDictionary<string, string> GetTags()
    {
        return saveableContainer.GetTags();
    }

    public void SetTags(IDictionary<string, string> tags)
    {
        saveableContainer.SetTags(tags);
    }

    private InventorySlot[] InitializeSlots()
    {
        InventorySlot[] slots = new InventorySlot[SlotCount];
        for (int i = 0; i < slots.Length; i++) slots[i] = new InventorySlot();
        return slots;
    }
}
