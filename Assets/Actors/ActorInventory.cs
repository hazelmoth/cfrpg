using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorInventory : MonoBehaviour {

	public delegate void GenericEvent();
	public delegate void InventoryEvent(Item[] inv, Item[] hotbar, Item[] apparel);
	public delegate void InventoryContainerEvent(InteractableContainer container);
	public delegate void HatEquipEvent(Hat hat);
	public delegate void ShirtEquipEvent(Shirt shirt);
	public delegate void PantsEquipEvent(Pants pants);
	public event GenericEvent OnInventoryChanged;
	public event InventoryEvent OnInventoryChangedLikeThis;
	public event InventoryContainerEvent OnCurrentContainerChanged;
	public event HatEquipEvent OnHatEquipped;
	public event ShirtEquipEvent OnShirtEquipped;
	public event PantsEquipEvent OnPantsEquipped;

	const int inventorySize = 18;
	const int hotbarSize = 6;

	Item[] inv;
	Item[] hotbar;
	Item hat;
	Item shirt;
	Item pants;
	InteractableContainer currentActiveContainer;

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
		}
	}

    public void Initialize ()
    {
        this.inv = new Item[inventorySize];
        this.hotbar = new Item[hotbarSize];
    }

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
	public void SetInventory(InvContents inv)
	{
		this.inv = inv.mainInvArray;
		hotbar = inv.hotbarArray;
		hat = inv.equippedHat;
		shirt = inv.equippedShirt;
		pants = inv.equippedPants;

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
			items.Add (item);
		}
		foreach (Item item in inv) {
			items.Add (item);
		}
		foreach (Item item in GetApparelArray()) {
			items.Add (item);
		}
		return items;
	}
	public Item GetItemInSlot (int slotNum, InventorySlotType slotType) {
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
    public InteractableContainer GetCurrentContainer() {
		return currentActiveContainer;
	}
	public bool ContainsAllItems (List<Item> items) {
		// Create a copy of the inv arrays, so we can remove elements as we test them
		Item[] testInv = (Item[])inv.Clone ();
		Item[] testHotbar = (Item[])hotbar.Clone ();
		Item[] testApparel = (Item[])GetApparelArray ().Clone ();

		foreach(Item item in items) {
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
	public bool RemoveOneInstanceOf (Item item) {
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
	
	
	public bool AttemptAddItemToInv (Item item) {
		for (int i = 0; i < hotbar.Length; i++) {
			if (hotbar[i] == null) {
				hotbar[i] = item;
				if (OnInventoryChangedLikeThis != null)
					OnInventoryChangedLikeThis (inv, hotbar, new Item[] {hat, shirt, pants});
				if (OnInventoryChanged != null)
					OnInventoryChanged ();
				return true;
			}
		}
		for (int i = 0; i < inv.Length; i++) {
			if (inv[i] == null) {
				inv[i] = item;
				if (OnInventoryChangedLikeThis != null)
					OnInventoryChangedLikeThis (inv, hotbar, new Item[] {hat, shirt, pants});
				if (OnInventoryChanged != null)
					OnInventoryChanged ();
				return true;
			}
		}
		return false;
	}
	public void AttemptMoveInventoryItem (int slot1, InventorySlotType typeSlot1, int slot2, InventorySlotType typeSlot2) {
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

		// If either slot is an apparel slot, make sure the other item is the appropriate apparel
		// Then perform that half of the switcharoo if this item fits in the other slot

		// Immediately cancel if the drag is between two apparel slots, since that can never work
		if (typeSlot1 == InventorySlotType.Hat || typeSlot1 == InventorySlotType.Shirt || typeSlot1 == InventorySlotType.Pants) {
			if (typeSlot2 == InventorySlotType.Hat || typeSlot2 == InventorySlotType.Shirt || typeSlot2 == InventorySlotType.Pants)
				return;
		}

		if (typeSlot1 == InventorySlotType.Hat) {
			Hat checkHat = item2 as Hat;
			if (checkHat == null && item2 != null)
				return;
			hat = item2;
			if (OnHatEquipped != null)
				OnHatEquipped (hat as Hat);
		}
		else if (typeSlot1 == InventorySlotType.Shirt) {
			Shirt checkShirt = item2 as Shirt;
			if (checkShirt == null && item2 != null)
				return;
			shirt = item2;
			if (OnShirtEquipped != null)
				OnShirtEquipped (shirt as Shirt);
		}
		else if (typeSlot1 == InventorySlotType.Pants) {
			Pants checkPants = item2 as Pants;
			if (checkPants == null && item2 != null)
				return;
			pants = item2;
			if (OnPantsEquipped != null)
				OnPantsEquipped (pants as Pants);
		}
		if (typeSlot2 == InventorySlotType.Hat) {
			Hat checkHat = item1 as Hat;
			if (checkHat == null)
				return;
			hat = item1;
			if (OnHatEquipped != null)
				OnHatEquipped (hat as Hat);
		}
		else if (typeSlot2 == InventorySlotType.Shirt) {
			Shirt checkShirt = item1 as Shirt;
			if (checkShirt == null)
				return;
			shirt = item1;
			if (OnShirtEquipped != null)
				OnShirtEquipped (shirt as Shirt);
		}
		else if (typeSlot2 == InventorySlotType.Pants) {
			Pants checkPants = item1 as Pants;
			if (checkPants == null)
				return;
			pants = item1;
			if (OnPantsEquipped != null)
				OnPantsEquipped (pants as Pants);
		}

		if (typeSlot1 == InventorySlotType.Inventory) {
			inv [slot1] = item2;
		}
		else if (typeSlot1 == InventorySlotType.Hotbar) {
			hotbar [slot1] = item2;
		}
		else if (typeSlot1 == InventorySlotType.ContainerInv) {
			currentActiveContainer.GetContainerInventory() [slot1] = item2;
		}

		if (typeSlot2 == InventorySlotType.Inventory) {
			inv [slot2] = item1;
		}
		else if (typeSlot2 == InventorySlotType.Hotbar) {
			hotbar [slot2] = item1;
		}
		else if (typeSlot2 == InventorySlotType.ContainerInv) {
			currentActiveContainer.GetContainerInventory() [slot2] = item1;
		}
		if (OnInventoryChangedLikeThis != null)
			OnInventoryChangedLikeThis (inv, hotbar, new Item[] {hat, shirt, pants});
		if (OnInventoryChanged != null)
			OnInventoryChanged ();
		if (OnCurrentContainerChanged != null)
			OnCurrentContainerChanged (currentActiveContainer);
		return;
	}

	public void DropInventoryItem (int slot, InventorySlotType type) {
		Debug.Log ("drop");
		Item item = GetItemInSlot (slot, type);
		if (item == null)
			return;

		ClearSlot (slot, type);
		DroppedItemSpawner.SpawnItem (item.ItemId, transform.localPosition, GetComponent<Actor>().ActorCurrentScene);

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
			if (OnHatEquipped != null)
				OnHatEquipped (hat as Hat);
		}
		else if (type == InventorySlotType.Shirt) {
            shirt = null;
			if (OnShirtEquipped != null)
				OnShirtEquipped (shirt as Shirt);
		}
		else if (type == InventorySlotType.Pants) {
            pants = null;
			if (OnPantsEquipped != null)
				OnPantsEquipped (pants as Pants);
		}
		else if (type == InventorySlotType.ContainerInv) {
			currentActiveContainer.GetContainerInventory () [slot] = null;
		}

		OnInventoryChangedLikeThis?.Invoke(inv, hotbar, new Item[] { hat, shirt, pants });
		OnInventoryChanged?.Invoke();
	}

	public void OnInteractWithContainer (InteractableObject interactable) {
		InteractableContainer container = interactable as InteractableContainer;
		if (container != null) {
			currentActiveContainer = container;
			if (OnCurrentContainerChanged != null)
				OnCurrentContainerChanged (container);
		}
	}
}
