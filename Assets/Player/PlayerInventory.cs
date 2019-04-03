using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour {

	public static PlayerInventory instance;
	public delegate void InventoryEvent(Item[] inv, Item[] hotbar, Item[] apparel);
	public delegate void InventoryContainerEvent(InteractableContainer container);
	public delegate void HatEquipEvent(Hat hat);
	public delegate void ShirtEquipEvent(Shirt shirt);
	public delegate void PantsEquipEvent(Pants pants);
	public static event InventoryEvent OnInventoryChanged;
	public static event InventoryContainerEvent OnCurrentContainerChanged;
	public static event HatEquipEvent OnHatEquipped;
	public static event ShirtEquipEvent OnShirtEquipped;
	public static event PantsEquipEvent OnPantsEquipped;

	const int inventorySize = 18;
	const int hotbarSize = 6;

	Item[] inv;
	Item[] hotbar;
	Item hat;
	Item shirt;
	Item pants;
	InteractableContainer currentActiveContainer;

    public void Initialize ()
    {
        instance = this;
        this.inv = new Item[inventorySize];
        this.hotbar = new Item[hotbarSize];

        InventoryScreenManager.OnInventoryDrag += AttemptMoveInventoryItem;
        InventoryScreenManager.OnInventoryDragOutOfWindow += DropInventoryItem;
        PlayerInteractionManager.OnPlayerInteract += OnPlayerInteract;
    }

    public static Item[] GetInventoryArray() {
		return instance.inv;
	}
	public static Item[] GetHotbarArray() {
		return instance.hotbar;
	}
	public static Item[] GetApparelArray() {
		return new Item[] {instance.hat, instance.shirt, instance.pants};
	}
	public static Item GetItemInSlot (int slotNum, InventorySlotType slotType) {
		if (slotType == InventorySlotType.Inventory)
			return instance.inv[slotNum];
		if (slotType == InventorySlotType.Hotbar)
			return instance.hotbar[slotNum];
		if (slotType == InventorySlotType.ContainerInv) {
			if (instance.currentActiveContainer == null)
				return null;
			return instance.currentActiveContainer.GetContainerInventory () [slotNum];
		}
		if (slotType == InventorySlotType.Hat)
			return instance.hat;
		if (slotType == InventorySlotType.Shirt)
			return instance.shirt;
		if (slotType == InventorySlotType.Pants)
			return instance.pants;
		return null;
	}
    public static Hat GetEquippedHat ()
    {
        return instance.hat as Hat;
    }
    public static Shirt GetEquippedShirt ()
    {
        return instance.shirt as Shirt;
    }
    public static Pants GetEquippedPants ()
    {
        return instance.pants as Pants;
    }
    public InteractableContainer GetCurrentContainer() {
		return currentActiveContainer;
	}
	public static bool AttemptAddItemToInv (Item item) {
		for (int i = 0; i < instance.hotbar.Length; i++) {
			if (instance.hotbar[i] == null) {
				instance.hotbar[i] = item;
				if (OnInventoryChanged != null)
					OnInventoryChanged (instance.inv, instance.hotbar, new Item[] {instance.hat, instance.shirt, instance.pants});
				return true;
			}
		}
		for (int i = 0; i < instance.inv.Length; i++) {
			if (instance.inv[i] == null) {
				instance.inv[i] = item;
				if (OnInventoryChanged != null)
					OnInventoryChanged (instance.inv, instance.hotbar, new Item[] {instance.hat, instance.shirt, instance.pants});
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
		if (OnInventoryChanged != null)
			OnInventoryChanged (inv, hotbar, new Item[] {hat, shirt, pants});
		if (OnCurrentContainerChanged != null)
			OnCurrentContainerChanged (currentActiveContainer);
		return;
	}

	public void DropInventoryItem (int slot, InventorySlotType type) {
		Debug.Log ("drop");
		Item item = GetItemInSlot (slot, type);
		Debug.Log (item);

		ClearSlot (slot, type);
		DroppedItemSpawner.SpawnItem (item.itemId, transform.localPosition, Player.instance.ActorCurrentScene);

		if (OnInventoryChanged != null)
			OnInventoryChanged (inv, hotbar, new Item[]{hat, shirt, pants});
	}

	void ClearSlot (int slot, InventorySlotType type) {
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
	}

	void OnPlayerInteract (InteractableObject thing) {
		InteractableContainer container = thing as InteractableContainer;
		if (container != null) {
			currentActiveContainer = container;
			if (OnCurrentContainerChanged != null)
				OnCurrentContainerChanged (container);
		}
	}
}
