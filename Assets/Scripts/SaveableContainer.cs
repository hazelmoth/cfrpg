using System.Collections.Generic;
using UnityEngine;

/// A base class for containers whose contents are saveable.
public abstract class SaveableContainer : MonoBehaviour, IContainer, ISaveable
{
    private SaveableContainerData container;

    public abstract string Name { get; }

    public int SlotCount => container.SlotCount;


    public virtual ItemStack Get(int slot)
    {
        container ??= new SaveableContainerData(InitializeSlots());

        return container.Get(slot);
    }

    public virtual void Set(int slot, ItemStack item)
    {
        container ??= new SaveableContainerData(InitializeSlots());

        container.Set(slot, item);
    }

    public bool AcceptsItemType(string itemId, int slot)
    {
        return container.AcceptsItemType(itemId, slot);
    }

    public abstract string ComponentId { get; }

    public virtual IDictionary<string, string> GetTags()
    {
        return container.GetTags();
    }

    public virtual void SetTags(IDictionary<string, string> tags)
    {
        container.SetTags(tags);
    }

    /// Initializes the contents of the slots array.
    /// Override this to define slots with specific properties.
    protected virtual InventorySlot[] InitializeSlots()
    {
        InventorySlot[] slots = new InventorySlot[SlotCount];
        for (int i = 0; i < slots.Length; i++) slots[i] = new InventorySlot();
        return slots;
    }
}
