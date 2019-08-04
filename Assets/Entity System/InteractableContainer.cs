using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableContainer : MonoBehaviour, InteractableObject {

	[SerializeField] string containerName;
	[SerializeField] protected int numSlots;
	protected Item[] inventory;

	public virtual void OnInteract () {}

	public virtual void ContentsWereChanged() {}

	public virtual bool CanHoldItem(Item item)
	{
		return true;
	}

	public virtual bool AttemptAddItem (Item item)
	{
		if (inventory == null)
			inventory = new Item[numSlots];

		for (int i = 0; i < numSlots; i++)
		{
			if (inventory[i] == null)
			{
				inventory[i] = item;
				ContentsWereChanged();
				return true;
			}
		}
		return false;
	}

	public virtual bool AttemptPlaceItemInSlot (Item item, int slot, bool ignoreItemAlreadyInSlot = false)
	{
		if (inventory == null)
			inventory = new Item[numSlots];

		if (ignoreItemAlreadyInSlot || inventory[slot] == null)
		{
			inventory[slot] = item;
			ContentsWereChanged();
			return true;
		}
		return false;
	}

	public Item[] GetContainerInventory () {
		if (inventory == null)
		{
			inventory = new Item[numSlots];
		}
		return inventory;
	}

	public int NumSlots {
		get {return numSlots;}
	}

	public string ContainerName {
		get {return containerName;}
	}

	
}
