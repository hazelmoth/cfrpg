using System;
using System.Collections.Generic;
using ContentLibraries;
using GUI.ContainerLayoutElements;
using Items;
using UnityEngine;

/// An interactable object for butchering corpses.
/// TODO: make butcher yield a property of races rather than a recipe.
public class ButcherTable : SaveableContainer, ICustomLayoutContainer, IInteractable
{
    [SerializeField] private string stationName = "Butcher Table";

    private Action<IContainer> updateListener;

    protected override string SavedComponentId => nameof(ButcherTable);

    public override string Name => stationName;

    public override int SlotCount => 5; // Input slot and 4 output slots

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

    protected override void InitializeSlots()
    {
        slots = new InventorySlot[SlotCount];

        // First slot only accepts corpses
        slots[0] = new InventorySlot(itemId => ItemIdParser.ParseBaseId(itemId) == "corpse");

        // Output slots don't accept anything from the outside
        for (int i = 1; i < SlotCount; i++)
            slots[i] = new InventorySlot(_ => false);

        updateListener?.Invoke(this);
    }

    private void ButcherInputItem()
    {
        if (slots[0].Empty) return;
        string itemId = ItemIdParser.ParseBaseId(slots[0].Contents.Id);
        if (itemId != "corpse") return;

        IDictionary<string, string> modifiers = ItemIdParser.ParseModifiers(slots[0].Contents.Id);
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
        ActorRace race = ContentLibrary.Instance.Races.Get(raceId);

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
                for (int i = 1; i < SlotCount; i++)
                {
                    if (slots[i].Empty)
                    {
                        slots[i].Contents = new ItemStack(id, 1);
                        placedItem = true;
                        break;
                    }
                    else if (slots[i].Contents.Id == id && slots[i].Contents.Quantity < itemData.MaxStackSize)
                    {
                        slots[i].Contents = slots[i].Contents.Incremented();
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
        slots[0].Contents = slots[0].Contents.Decremented();

        updateListener?.Invoke(this);
    }
}
