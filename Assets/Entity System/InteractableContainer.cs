using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableContainer : MonoBehaviour, InteractableObject {

	public delegate void ContainerEvent();
	public delegate void DetailedContainerEvent(InteractableContainer container);
	public static ContainerEvent SomeContainerDestroyed;
	public static DetailedContainerEvent ContainerDestroyed;

	static bool hasSetUpSceneChangeHandler = false;

	[SerializeField] string containerName;
	[SerializeField] protected int numSlots;
	protected Item[] inventory;

	static void ResetStaticMembers ()
	{
		hasSetUpSceneChangeHandler = false;
		SomeContainerDestroyed = null;
		ContainerDestroyed = null;
	}

	void Start ()
	{
		if (!hasSetUpSceneChangeHandler)
		{
			SceneChangeManager.OnSceneExit += ResetStaticMembers;
			hasSetUpSceneChangeHandler = true;
		}
	}

	void OnDestroy()
	{
		ContainerDestroyed?.Invoke(this);
	}

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
	public int NumFullSlots
	{
		get
		{
			int fullSlots = 0;
			foreach (Item item in inventory)
			{
				if (item != null)
					fullSlots++;
			}
			return fullSlots;
		}
	}

	public string ContainerName {
		get {return containerName;}
	}

	
}
