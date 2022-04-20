using System;
using System.Collections.Generic;
using ContentLibraries;
using GUI.ContainerLayoutElements;
using Items;
using UnityEngine;

/// An interactable object for butchering corpses.
public class ButcherTable : SaveableContainer, ICustomLayoutContainer, IInteractableContainer
{
    private new const int SlotCount = 5;

    [SerializeField] private string stationName = "Butcher Table";

    private Action<IContainer> updateListener;

    public override string ComponentId => nameof(ButcherTable);

    public override string Name => stationName;

    public override void Set(int slot, ItemStack item)
    {
        base.Set(slot, item);
        updateListener?.Invoke(this);
    }

    public List<IContainerLayoutElement> GetLayoutElements()
    {
        // TODO define a container layout element for a button!
        return new List<IContainerLayoutElement>
        {
            new ContainerLayoutLabel(() => "Input"),
            new ContainerLayoutInvArray(0, 0),
            new ContainerLayoutButton(() => "Butcher", ButcherInputItem),
            new ContainerLayoutLabel(() => "Output"),
            new ContainerLayoutInvArray(1, 4),
        };
    }

    void ICustomLayoutContainer.SetUpdateListener(Action<IContainer> listener)
    {
        updateListener = listener;
    }

    public void OnInteract() { }

    public override void SetTags(IDictionary<string, string> tags)
    {
        base.SetTags(tags);
        updateListener?.Invoke(this);
    }

    protected override InventorySlot[] InitializeSlots()
    {
        InventorySlot[] slots = new InventorySlot[SlotCount];

        // First slot only accepts corpses
        slots[0] = new InventorySlot(itemId => ItemIdParser.ParseBaseId(itemId) == "corpse");

        // Output slots don't accept anything from the outside
        for (int i = 1; i < base.SlotCount; i++)
            slots[i] = new InventorySlot(_ => false);

        updateListener?.Invoke(this);
        return slots;
    }

    private void ButcherInputItem()
    {
        if (Get(0) == null) return;
        string itemId = ItemIdParser.ParseBaseId(Get(0).Id);
        if (itemId != "corpse") return;

        IDictionary<string, string> modifiers = ItemIdParser.ParseModifiers(Get(0).Id);
        if (!modifiers.ContainsKey("race"))
        {
            Debug.LogWarning("Corpse item missing race modifier. Can't butcher.");
            return;
        }

        string raceId = modifiers["race"];
        if (!ContentLibrary.Instance.Races.Contains(raceId))
        {
            Debug.LogWarning($"Corpse item has unknown race {raceId}. Can't butcher.");
            return;
        }
        IActorRace race = ContentLibrary.Instance.Races.Get(raceId);

        List<string> yield = race.ButcherDrops.Pick();
        yield.ForEach(
            id =>
            {
                if (!ContentLibrary.Instance.Items.Contains(id))
                {
                    Debug.LogError("Butcher drop yielded item with unrecognized id: " + id);
                    return;
                }
                ItemData itemData = ContentLibrary.Instance.Items.Get(id);

                bool placedItem = false;
                for (int i = 1; i < base.SlotCount; i++)
                {
                    if (Get(i) == null)
                    {
                        Set(i, new ItemStack(id, 1));
                        placedItem = true;
                        break;
                    }
                    else if (Get(i).Id == id && Get(i).Quantity < itemData.MaxStackSize)
                    {
                        Set(i, Get(i).Decremented());
                        placedItem = true;
                        break;
                    }
                }
                if (!placedItem)
                {
                    // All output slots are full, so drop the item.
                    DroppedItemSpawner.SpawnItem(
                        new ItemStack(id, 1),
                        GetComponent<EntityObject>().Location.Vector2,
                        GetComponent<EntityObject>().Location.scene,
                        true);
                }
            });
        Set(0, Get(0).Decremented());

        updateListener?.Invoke(this);
    }
}
