using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class ActorInventory {

	public delegate void GenericEvent();
	public delegate void InventoryEvent(ItemData[] inv, ItemData[] hotbar, ItemData[] apparel);
	public delegate void InventoryContainerEvent(InteractableContainer container);
	public delegate void HatEquipEvent(Hat hat);
	public delegate void ShirtEquipEvent(Shirt shirt);
	public delegate void PantsEquipEvent(Pants pants);
	public event GenericEvent OnInventoryChanged;
	public event GenericEvent OnActiveContainerDestroyedOrNull;
	public event InventoryEvent OnInventoryChangedLikeThis;
	public event InventoryContainerEvent OnCurrentContainerChanged;
	public event HatEquipEvent OnHatEquipped;
	public event ShirtEquipEvent OnShirtEquipped;
	public event PantsEquipEvent OnPantsEquipped;

	private const int inventorySize = 18;
	private const int hotbarSize = 6;

	private ItemData[] inv;
	private ItemData[] hotbar;
	private ItemData hat;
	private ItemData shirt;
	private ItemData pants;
	private InteractableContainer currentActiveContainer;

	// Using -1 for no slot equipped
	private int equippedHotbarSlot;

	public class InvContents
	{
		public ItemData[] mainInvArray;
		public ItemData[] hotbarArray;
		public ItemData equippedHat;
		public ItemData equippedShirt;
		public ItemData equippedPants;

		public InvContents()
		{
			mainInvArray = new ItemData[inventorySize];
			hotbarArray = new ItemData[hotbarSize];
		}
	}

    public void Initialize ()
    {
        this.inv = new ItemData[inventorySize];
        this.hotbar = new ItemData[hotbarSize];
		InteractableContainer.ContainerDestroyed += OnSomeContainerDestroyed;
		AttemptAddItemToInv(ContentLibrary.Instance.Items.GetItemById("birthday_hat"));
	}

	public int EquippedHotbarSlot => equippedHotbarSlot;
	public InvContents GetContents()
	{
		InvContents contents = new InvContents();
		contents.mainInvArray = GetMainInventoryArray();
		contents.hotbarArray = GetHotbarArray();
		contents.equippedHat = hat;
		contents.equippedShirt = shirt;
		contents.equippedPants = pants;
		return contents;
	}
	public void SetInventory([NotNull] InvContents inv)
	{
		this.inv = inv.mainInvArray;
		hotbar = inv.hotbarArray;
		hat = inv.equippedHat;
		shirt = inv.equippedShirt;
		pants = inv.equippedPants;

		OnHatEquipped?.Invoke(hat as Hat);
		OnShirtEquipped?.Invoke(shirt as Shirt);
		OnPantsEquipped?.Invoke(pants as Pants);
		OnInventoryChangedLikeThis?.Invoke(this.inv, hotbar, new ItemData[] { hat, shirt, pants });
		OnInventoryChanged?.Invoke();
	}
	public ItemData[] GetMainInventoryArray() {
		return inv;
	}
	public ItemData[] GetHotbarArray() {
		return hotbar;
	}
	public ItemData[] GetApparelArray() {
		return new ItemData[] {hat, shirt, pants};
	}
	public List<ItemData> GetAllItems () {
		List<ItemData> items = new List<ItemData> ();
		foreach (ItemData item in hotbar) {
			items.Add (item);
		}
		foreach (ItemData item in inv) {
			items.Add (item);
		}
		foreach (ItemData item in GetApparelArray()) {
			items.Add (item);
		}
		return items;
	}
	public ItemData GetItemInSlot (int slotNum, InventorySlotType slotType) {
		if (slotType == InventorySlotType.Inventory)
			return inv[slotNum];
		if (slotType == InventorySlotType.Hotbar)
			return hotbar[slotNum];
		if (slotType == InventorySlotType.ContainerInv) {
			if (currentActiveContainer == null)
				return null;
			return currentActiveContainer.GetContainerInventory () [slotNum];
		}
		if (slotType == InventorySlotType.Hat)
			return hat;
		if (slotType == InventorySlotType.Shirt)
			return shirt;
		if (slotType == InventorySlotType.Pants)
			return pants;
		return null;
	}
    public Hat GetEquippedHat ()
    {
        return hat as Hat;
    }
    public Shirt GetEquippedShirt ()
    {
        return shirt as Shirt;
    }
    public Pants GetEquippedPants ()
    {
        return pants as Pants;
    }
	public ItemData GetEquippedItem ()
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
	public bool ContainsAllItems (List<ItemData> items) {
		// Create a copy of the inv arrays, so we can remove elements as we test them
		ItemData[] testInv = (ItemData[])inv.Clone ();
		ItemData[] testHotbar = (ItemData[])hotbar.Clone ();
		ItemData[] testApparel = (ItemData[])GetApparelArray ().Clone ();

		foreach(ItemData item in items) {
			int i = Array.IndexOf (testInv, item);
			if (i >= 0) {
				testInv [i] = null;
				continue;
			}
			int j = Array.IndexOf (testHotbar, item);
			if (j >= 0) {
				testHotbar [j] = null;
				continue;
			}
			int k = Array.IndexOf (testApparel, item);
			if (k >= 0) {
				testApparel [k] = null;
				continue;
			}
			// Return false if an item hasn't been found in any array
			return false;
		}
		return true;
	}
	public bool RemoveOneInstanceOf (ItemData item) {
		int i = Array.IndexOf (inv, item);
		if (i >= 0) {
			ClearSlot (i, InventorySlotType.Inventory);
			return true;
		}
		int j = Array.IndexOf (hotbar, item);
		if (j >= 0) {
			ClearSlot (j, InventorySlotType.Hotbar);
			return true;
		}
		int k = Array.IndexOf (GetApparelArray(), item);
		if (k >= 0) {
			if (k == 0) {
				ClearSlot (0, InventorySlotType.Hat);
			} else if (k == 1) {
				ClearSlot (0, InventorySlotType.Shirt);
			} else if (k == 2) {
				ClearSlot (0, InventorySlotType.Pants);
			}
			return true;
		}
		return false;
	}
	public bool AttemptAddItemToInv (ItemData item) {
		for (int i = 0; i < hotbar.Length; i++) {
			if (hotbar[i] == null) {
				hotbar[i] = item;
				OnInventoryChangedLikeThis?.Invoke(inv, hotbar, new ItemData[] { hat, shirt, pants });
				OnInventoryChanged?.Invoke();
				return true;
			}
		}
		for (int i = 0; i < inv.Length; i++) {
			if (inv[i] == null) {
				inv[i] = item;
				OnInventoryChangedLikeThis?.Invoke(inv, hotbar, new ItemData[] { hat, shirt, pants });
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
		ItemData item1 = null, item2 = null;

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

		

		// Immediately cancel if the drag is between two apparel slots, since that can never work
		if (typeSlot1 == InventorySlotType.Hat || typeSlot1 == InventorySlotType.Shirt || typeSlot1 == InventorySlotType.Pants) {
			if (typeSlot2 == InventorySlotType.Hat || typeSlot2 == InventorySlotType.Shirt || typeSlot2 == InventorySlotType.Pants)
				return;
		}

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

		// If either slot is an apparel slot, make sure the other item is the appropriate apparel
		// Then perform that half of the switcharoo if this item fits in the other slot

		if (typeSlot1 == InventorySlotType.Hat) {
			Hat checkHat = item2 as Hat;
			if (checkHat == null && item2 != null)
				return;
			hat = item2;
			OnHatEquipped?.Invoke(hat as Hat);
		}
		else if (typeSlot1 == InventorySlotType.Shirt) {
			Shirt checkShirt = item2 as Shirt;
			if (checkShirt == null && item2 != null)
				return;
			shirt = item2;
			OnShirtEquipped?.Invoke(shirt as Shirt);
		}
		else if (typeSlot1 == InventorySlotType.Pants) {
			Pants checkPants = item2 as Pants;
			if (checkPants == null && item2 != null)
				return;
			pants = item2;
			OnPantsEquipped?.Invoke(pants as Pants);
		}
		if (typeSlot2 == InventorySlotType.Hat) {
			Hat checkHat = item1 as Hat;
			if (checkHat == null)
				return;
			hat = item1;
			OnHatEquipped?.Invoke(hat as Hat);
		}
		else if (typeSlot2 == InventorySlotType.Shirt) {
			Shirt checkShirt = item1 as Shirt;
			if (checkShirt == null)
				return;
			shirt = item1;
			OnShirtEquipped?.Invoke(shirt as Shirt);
		}
		else if (typeSlot2 == InventorySlotType.Pants) {
			Pants checkPants = item1 as Pants;
			if (checkPants == null)
				return;
			pants = item1;
			OnPantsEquipped?.Invoke(pants as Pants);
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
			// Cancel if this container refuses to accept this item
			if (!currentActiveContainer.AttemptPlaceItemInSlot(item2, slot1, true))
				return;
		}

		// Item 1 to slot 2:
		if (typeSlot2 == InventorySlotType.Inventory) {
			inv [slot2] = item1;
		}
		else if (typeSlot2 == InventorySlotType.Hotbar) {
			hotbar [slot2] = item1;
		}
		else if (typeSlot2 == InventorySlotType.ContainerInv) {
			// Cancel if this container refuses to accept this item
			if (!currentActiveContainer.AttemptPlaceItemInSlot(item1, slot2, true))
				return;
		}
		OnInventoryChangedLikeThis?.Invoke(inv, hotbar, new ItemData[] { hat, shirt, pants });
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
			ItemData item = GetAllItems()[i];
			if (item != null && item.ItemId == itemId)
			{
				if (container.AttemptAddItem(item))
					RemoveOneInstanceOf(item);
			}
		}
	}
	public void DropInventoryItem (int slot, InventorySlotType type, Vector2 scenePosition, string scene) {
		Debug.Log ("drop");
		ItemData item = GetItemInSlot (slot, type);
		if (item == null)
			return;

		ClearSlot (slot, type);
		DroppedItemSpawner.SpawnItem (item.ItemId, scenePosition, scene);

		OnInventoryChangedLikeThis?.Invoke(inv, hotbar, new ItemData[] { hat, shirt, pants });
		OnInventoryChanged?.Invoke();
	}
	public void ClearSlot (int slot, InventorySlotType type) {
		if (type == InventorySlotType.Inventory)
			inv [slot] = null;
		else if (type == InventorySlotType.Hotbar)
			hotbar [slot] = null;
		else if (type == InventorySlotType.Hat) {
			hat = null;
			OnHatEquipped?.Invoke(hat as Hat);
		}
		else if (type == InventorySlotType.Shirt) {
            shirt = null;
			OnShirtEquipped?.Invoke(shirt as Shirt);
		}
		else if (type == InventorySlotType.Pants) {
            pants = null;
			OnPantsEquipped?.Invoke(pants as Pants);
		}
		else if (type == InventorySlotType.ContainerInv) {
			currentActiveContainer.GetContainerInventory () [slot] = null;
			currentActiveContainer.ContentsWereChanged();
		}

		OnInventoryChangedLikeThis?.Invoke(inv, hotbar, new ItemData[] { hat, shirt, pants });
		OnInventoryChanged?.Invoke();
	}
	public void OnInteractWithContainer (InteractableObject interactable) {
		InteractableContainer container = interactable as InteractableContainer;
		if (container != null) {
			currentActiveContainer = container;
			OnCurrentContainerChanged?.Invoke(container);
		}
	}

	private void OnSomeContainerDestroyed (InteractableContainer container)
	{
		if (container == currentActiveContainer || currentActiveContainer == null || currentActiveContainer.gameObject == null)
		{
			OnActiveContainerDestroyedOrNull?.Invoke();
		}
	}
}
