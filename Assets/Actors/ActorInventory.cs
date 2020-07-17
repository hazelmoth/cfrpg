using System;
using System.Collections;
using System.Collections.Generic;
using Items;
using JetBrains.Annotations;
using UnityEngine;

public class ActorInventory {

	public delegate void InventoryEvent(Item[] inv, Item[] hotbar, Item[] apparel);
	public delegate void InventoryContainerEvent(InteractableContainer container);
	public delegate void HatEquipEvent(Item hat);
	public delegate void ShirtEquipEvent(Item shirt);
	public delegate void PantsEquipEvent(Item pants);
	public event Action OnInventoryChanged;
	public event Action OnActiveContainerDestroyedOrNull;
	public event InventoryEvent OnInventoryChangedLikeThis;
	public event InventoryContainerEvent OnCurrentContainerChanged;
	public event HatEquipEvent OnHatEquipped;
	public event ShirtEquipEvent OnShirtEquipped;
	public event PantsEquipEvent OnPantsEquipped;

	private const int inventorySize = 18;
	private const int hotbarSize = 6;

	private Item[] inv;
	private Item[] hotbar;
	private Item hat;
	private Item shirt;
	private Item pants;
	private InteractableContainer currentActiveContainer;

	// Using -1 for no slot equipped
	private int equippedHotbarSlot;

	[System.Serializable]
	public class InvContents
	{
		public Item[] mainInvArray;
		public Item[] hotbarArray;
		public Item equippedHat;
		public Item equippedShirt;
		public Item equippedPants;

		public InvContents()
		{
			mainInvArray = new Item[inventorySize];
			hotbarArray = new Item[hotbarSize];

			// Set these fields to null, since Unity initializes serializable classes as not null (wtf unity?)
			for (int i = 0; i < mainInvArray.Length; i++)
			{
				mainInvArray[i] = null;
			}
			for (int i = 0; i < hotbarArray.Length; i++)
			{
				hotbarArray[i] = null;
			}
			equippedHat = null;
			equippedPants = null;
		}
	}

	public ActorInventory ()
	{
		Initialize();
	}
    public void Initialize ()
    {
        this.inv = new Item[inventorySize];
        this.hotbar = new Item[hotbarSize];

		SetInventory(ReplaceBlankItemsWithNull(GetContents()));

		InteractableContainer.ContainerDestroyed += OnSomeContainerDestroyed;
	}

	public int EquippedHotbarSlot => equippedHotbarSlot;

	// Returns an InvContents object with a *copy* of this inventory's contents
	public InvContents GetContents()
	{
		InvContents contents = new InvContents();
		contents.mainInvArray = GetMainInventoryArray();
		contents.hotbarArray = GetHotbarArray();
		contents.equippedHat = hat;
		contents.equippedShirt = shirt;
		contents.equippedPants = pants;
		contents = ReplaceBlankItemsWithNull(contents);
		return contents;
	}
	public void SetInventory([NotNull] InvContents inv)
	{
		if (inv.mainInvArray == null)
		{
			inv.mainInvArray = new Item[inventorySize];
		}
		if (inv.hotbarArray == null)
		{
			inv.hotbarArray = new Item[hotbarSize];
		}

		inv = ReplaceBlankItemsWithNull(inv);

		this.inv = inv.mainInvArray;
		hotbar = inv.hotbarArray;
		hat = inv.equippedHat;
		shirt = inv.equippedShirt;
		pants = inv.equippedPants;

		OnHatEquipped?.Invoke(hat);
		OnShirtEquipped?.Invoke(shirt);
		OnPantsEquipped?.Invoke(pants);
		OnInventoryChangedLikeThis?.Invoke(this.inv, hotbar, new Item[] { hat, shirt, pants });
		OnInventoryChanged?.Invoke();
	}
	public Item[] GetMainInventoryArray() {
		return inv;
	}
	public Item[] GetHotbarArray() {
		return hotbar;
	}
	public Item[] GetApparelArray() {
		return new Item[] {hat, shirt, pants};
	}
	public List<Item> GetAllItems () {
		List<Item> items = new List<Item> ();
		foreach (Item item in hotbar) {
			if (item != null)
				items.Add (item);
		}
		foreach (Item item in inv) {
			if (item != null)
				items.Add (item);
		}
		foreach (Item item in GetApparelArray()) {
			if (item != null)
				items.Add (item);
		}
		return items;
	}
	public Item GetItemInSlot (int slotNum, InventorySlotType slotType) 
	{
		Item result;

		if (slotType == InventorySlotType.Inventory)
			result = inv[slotNum];
		else if (slotType == InventorySlotType.Hotbar)
			result = hotbar[slotNum];
		else if (slotType == InventorySlotType.ContainerInv) {
			if (currentActiveContainer == null)
				result = null;
			result = currentActiveContainer.GetContainerInventory () [slotNum];
		}
		else if (slotType == InventorySlotType.Hat)
			result = hat;
		else if (slotType == InventorySlotType.Shirt)
			result = shirt;
		else if (slotType == InventorySlotType.Pants)
			result = pants;
		else
			result = null;

		// Make sure unity's dumbass serialization hasn't given us a blank item
		if (result != null && result.id == null)
		{
			result = null;
		}
		return result;
	}
    public Item GetEquippedHat ()
    {
		return hat;
    }
    public Item GetEquippedShirt ()
    {
		return shirt;
    }
    public Item GetEquippedPants ()
    {
		return pants;
    }
	public Item GetEquippedItem ()
	{
		if (equippedHotbarSlot >= 0 && equippedHotbarSlot < hotbarSize)
		{
			return hotbar[equippedHotbarSlot];
		}
		else
		{
			return null;
		}
	}
    public InteractableContainer GetCurrentContainer() {
		return currentActiveContainer;
	}
	public int GetCountOf(string itemId)
	{
		int count = 0;
		foreach (Item item in GetAllItems())
		{
			if (item.id == itemId)
			{
				count += item.quantity;
			}
		}
		return count;
	}
	public bool IsFull (bool includeApparelSlots = false)
	{
		for (int i = 0; i < inv.Length; i++)
		{
			if (inv[i] == null)
				return false;
		}
		for (int i = 0; i < hotbar.Length; i++)
		{
			if (hotbar[i] == null)
				return false;
		}
		if (includeApparelSlots)
		{
			if (hat == null || shirt == null || pants == null)
				return false;
		}
		return true;
	}

	// Returns true if this inventory stores every item in the quantity present in
	// the list; i.e., if the list has an item twice, the inv must have at least two
	// of that item.
	public bool ContainsAllItems (List<string> ids) {
		// Create a copy of the inv arrays, so we can remove elements as we test them
		InvContents contents = GetContents();

		foreach(string itemId in ids) {
			int i = Array.FindIndex (contents.mainInvArray, (Item it) => it != null && it.id == itemId);
			if (i >= 0) {
				contents.mainInvArray [i] = null;
				continue;
			}
			i = Array.FindIndex(contents.hotbarArray, (Item it) => it != null && it.id == itemId);
			if (i >= 0) {
				contents.hotbarArray [i] = null;
				continue;
			}
			if (contents.equippedHat != null && contents.equippedHat.id == itemId) {
				contents.equippedHat = null;
				continue;
			}
			if (contents.equippedShirt != null && contents.equippedShirt.id == itemId) {
				contents.equippedShirt = null;
				continue;
			}
			if (contents.equippedPants != null && contents.equippedPants.id == itemId) {
				contents.equippedPants = null;
				continue;
			}
			// Return false if an item hasn't been found in any array
			return false;
		}
		return true;
	}
	public bool RemoveOneInstanceOf (string itemId) 
	{
		int i = Array.FindIndex(inv, (Item it) => it != null && it.id == itemId);
		if (i >= 0) {
			inv[i] = DecrementStack(inv[i]);
			return true;
		}

		i = Array.FindIndex(hotbar, (Item it) => it != null && it.id == itemId);
		if (i >= 0) {
			hotbar[i] = DecrementStack(hotbar[i]);
			return true;
		}

		i = Array.FindIndex(GetApparelArray(), (Item it) => it != null && it.id == itemId);
		if (i >= 0) {
			if (i == 0) {
				hat = DecrementStack(hat);
			} else if (i == 1) {
				shirt = DecrementStack(shirt);
			} else if (i == 2) {
				pants = DecrementStack(pants);
			}
			return true;
		}
		return false;
	}

	// Removes the given quantity of the specified item. Returns true if all
	// the items were successfully found and removed.
	public bool Remove(string itemId, int count)
	{
		bool success = true;
		for (int i = 0; i < count; i++)
		{
			if (!RemoveOneInstanceOf(itemId))
			{
				success = false;
			}
		}

		return success;
	}
	public bool AttemptAddItemToInv (Item item) {
		for (int i = 0; i < hotbar.Length; i++) {
			if (hotbar[i] == null) {
				hotbar[i] = item;
				OnInventoryChangedLikeThis?.Invoke(inv, hotbar, new Item[] { hat, shirt, pants });
				OnInventoryChanged?.Invoke();
				return true;
			}
		}
		for (int i = 0; i < inv.Length; i++) {
			if (inv[i] == null) {
				inv[i] = item;
				OnInventoryChangedLikeThis?.Invoke(inv, hotbar, new Item[] { hat, shirt, pants });
				OnInventoryChanged?.Invoke();
				return true;
			}
		}
		return false;
	}
	public void SetEquippedHotbarSlot (int slot)
	{
		equippedHotbarSlot = slot;
	}
	public void ClearEquippedHotbarSlot ()
	{
		equippedHotbarSlot = -1;
	}
	public void AttemptMoveInventoryItem (int slot1, InventorySlotType typeSlot1, int slot2, InventorySlotType typeSlot2) {
		SetInventory(ReplaceBlankItemsWithNull(GetContents()));
		Item item1 = null, item2 = null;

		if (typeSlot1 == InventorySlotType.Inventory)
			item1 = inv [slot1];
		else if (typeSlot1 == InventorySlotType.Hotbar)
			item1 = hotbar [slot1];
		else if (typeSlot1 == InventorySlotType.Hat)
			item1 = hat;
		else if (typeSlot1 == InventorySlotType.Shirt)
			item1 = shirt;
		else if (typeSlot1 == InventorySlotType.Pants)
			item1 = pants;
		else if (typeSlot1 == InventorySlotType.ContainerInv)
			item1 = currentActiveContainer.GetContainerInventory()[slot1];
		if (item1 == null)
			return;

		if (typeSlot2 == InventorySlotType.Inventory)
			item2 = inv [slot2];
		else if (typeSlot2 == InventorySlotType.Hotbar)
			item2 = hotbar [slot2];
		else if (typeSlot2 == InventorySlotType.Hat)
			item2 = hat;
		else if (typeSlot2 == InventorySlotType.Shirt)
			item2 = shirt;
		else if (typeSlot2 == InventorySlotType.Pants)
			item2 = pants;
		else if (typeSlot2 == InventorySlotType.ContainerInv)
			item2 = currentActiveContainer.GetContainerInventory()[slot2];


		// If either slot is a container slot, make sure it will accept the item we're trying to put in it

		if (typeSlot1 == InventorySlotType.ContainerInv)
		{
			if (!currentActiveContainer.CanHoldItem(item2))
			{
				return;
			}
		}
		if (typeSlot2 == InventorySlotType.ContainerInv)
		{
			if (!currentActiveContainer.CanHoldItem(item1))
			{
				return;
			}
		}

		// Make sure the slot types are otherwise compatible

		if (!SlotTypeIsCompatible(typeSlot2, item1) || !SlotTypeIsCompatible(typeSlot1, item2))
		{
			return;
		}

		// If either slot is an apparel slot, perform that half of the switcheroo

		if (typeSlot1 == InventorySlotType.Hat) {
			hat = item2;
			OnHatEquipped?.Invoke(hat);
		}
		else if (typeSlot1 == InventorySlotType.Shirt) {
			shirt = item2;
			OnShirtEquipped?.Invoke(shirt);
		}
		else if (typeSlot1 == InventorySlotType.Pants) {
			pants = item2;
			OnPantsEquipped?.Invoke(pants);
		}

		if (typeSlot2 == InventorySlotType.Hat) {
			hat = item1;
			OnHatEquipped?.Invoke(hat);
		}
		else if (typeSlot2 == InventorySlotType.Shirt) {
			shirt = item1;
			OnShirtEquipped?.Invoke(shirt);
		}
		else if (typeSlot2 == InventorySlotType.Pants) {
			pants = item1;
			OnPantsEquipped?.Invoke(pants);
		}

		// For any other slot types, just go ahead and switch 'em

		// Item 2 to slot 1:
		if (typeSlot1 == InventorySlotType.Inventory) {
			inv [slot1] = item2;
		}
		else if (typeSlot1 == InventorySlotType.Hotbar) {
			hotbar [slot1] = item2;
		}
		else if (typeSlot1 == InventorySlotType.ContainerInv) {
			currentActiveContainer.AttemptPlaceItemInSlot(item2, slot1, true);
		}

		// Item 1 to slot 2:
		if (typeSlot2 == InventorySlotType.Inventory) {
			inv [slot2] = item1;
		}
		else if (typeSlot2 == InventorySlotType.Hotbar) {
			hotbar [slot2] = item1;
		}
		else if (typeSlot2 == InventorySlotType.ContainerInv) {
			currentActiveContainer.AttemptPlaceItemInSlot(item1, slot2, true);
		}
		OnInventoryChangedLikeThis?.Invoke(inv, hotbar, new Item[] { hat, shirt, pants });
		OnInventoryChanged?.Invoke();
		OnCurrentContainerChanged?.Invoke(currentActiveContainer);
		return;
	}
	public void TransferMatchingItemsToContainer(string itemId, InteractableContainer container)
	{
		currentActiveContainer = container;
		OnCurrentContainerChanged?.Invoke(container);

		for (int i = GetAllItems().Count - 1; i >= 0; i--)
		{
			Item item = GetAllItems()[i];
			if (item != null && item.id == itemId)
			{
				if (container.AttemptAddItem(item))
					RemoveOneInstanceOf(item.id);
			}
		}
	}
	public void DropInventoryItem (int slot, InventorySlotType type, Vector2 scenePosition, string scene) {
		Item item = GetItemInSlot (slot, type);
		if (item == null)
			return;

		ClearSlot (slot, type);
		DroppedItemSpawner.SpawnItem (item.id, scenePosition, scene);

		OnInventoryChangedLikeThis?.Invoke(inv, hotbar, new Item[] { hat, shirt, pants });
		OnInventoryChanged?.Invoke();
	}
	public void ClearSlot (int slot, InventorySlotType type) {
		if (type == InventorySlotType.Inventory)
			inv [slot] = null;
		else if (type == InventorySlotType.Hotbar)
			hotbar [slot] = null;
		else if (type == InventorySlotType.Hat) {
			hat = null;
			OnHatEquipped?.Invoke(hat);
		}
		else if (type == InventorySlotType.Shirt) {
            shirt = null;
			OnShirtEquipped?.Invoke(shirt);
		}
		else if (type == InventorySlotType.Pants) {
            pants = null;
			OnPantsEquipped?.Invoke(pants);
		}
		else if (type == InventorySlotType.ContainerInv) {
			currentActiveContainer.GetContainerInventory () [slot] = null;
			currentActiveContainer.ContentsWereChanged();
		}

		OnInventoryChangedLikeThis?.Invoke(inv, hotbar, new Item[] { hat, shirt, pants });
		OnInventoryChanged?.Invoke();
	}
	public void OnInteractWithContainer (IInteractableObject interactable) {
		InteractableContainer container = interactable as InteractableContainer;
		if (container != null) {
			currentActiveContainer = container;
			OnCurrentContainerChanged?.Invoke(container);
		}
	}

	private bool SlotTypeIsCompatible (InventorySlotType type, Item item)
	{
		// Any slot can be empty
		if (item == null) {
			return true; 
		}

		if (type == InventorySlotType.Hat)
		{
			return item.GetData() is IHat;
		}
		else if (type == InventorySlotType.Shirt)
		{
			return item.GetData() is Shirt;
		}
		else if (type == InventorySlotType.Pants)
		{
			return (item.GetData() is Pants);
		}
		// All other slot types can hold any item
		return true;
	}
	private void OnSomeContainerDestroyed (InteractableContainer container)
	{
		if (container == currentActiveContainer || currentActiveContainer == null || currentActiveContainer.gameObject == null)
		{
			OnActiveContainerDestroyedOrNull?.Invoke();
		}
	}

	// Returns the given stack with one fewer item, or null if the item count hits 0.
	private static Item DecrementStack(Item stack)
	{
		stack.quantity -= 1;
		if (stack.quantity <= 0)
		{
			return null;
		}
		return stack;
	}

	// Replaces any blank IDs in the given inventory with null items and returns the inventory.
	private static InvContents ReplaceBlankItemsWithNull(InvContents inv)
	{
		for (int i = 0; i < inv.mainInvArray.Length; i++)
		{
			if (inv.mainInvArray[i] != null && string.IsNullOrEmpty(inv.mainInvArray[i].id))
			{
				inv.mainInvArray[i] = null;
			}
		}
		for (int i = 0; i < inv.hotbarArray.Length; i++)
		{
			if (inv.hotbarArray[i] != null && string.IsNullOrEmpty(inv.hotbarArray[i].id))
			{
				inv.hotbarArray[i] = null;
			}
		}
		inv.equippedHat = (inv.equippedHat != null && string.IsNullOrEmpty(inv.equippedHat.id)) ? null : inv.equippedHat;
		inv.equippedShirt = (inv.equippedShirt != null && string.IsNullOrEmpty(inv.equippedShirt.id)) ? null : inv.equippedShirt;
		inv.equippedPants = (inv.equippedPants != null && string.IsNullOrEmpty(inv.equippedPants.id)) ? null : inv.equippedPants;

		return inv;
	}
}
