using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableContainer : SaveableComponent, IInteractableObject {

	public delegate void ContainerEvent();
	public delegate void DetailedContainerEvent(InteractableContainer container);
	public static ContainerEvent SomeContainerDestroyed;
	public static DetailedContainerEvent ContainerDestroyed;

	private static bool hasSetUpSceneChangeHandler = false;

	[SerializeField] private string containerName;
	[SerializeField] protected int numSlots;
	protected Item[] inventory;

	private static void ResetStaticMembers ()
	{
		hasSetUpSceneChangeHandler = false;
		SomeContainerDestroyed = null;
		ContainerDestroyed = null;
	}

	private void Start ()
	{
		if (!hasSetUpSceneChangeHandler)
		{
			SceneChangeActivator.OnSceneExit += ResetStaticMembers;
			hasSetUpSceneChangeHandler = true;
		}
	}

	private void OnDestroy()
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
	public bool IsFull
	{
		get {
			return (NumFullSlots == 0);
		}
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


	// SAVE TAGS
	// container name, 
	// number of slots,
	// inv contents (1 tag per slot)

	public override void SetTags(List<string> tags)
	{
		containerName = tags[0];
		numSlots = int.Parse(tags[1]);
		tags.RemoveRange(0, 2);

        inventory = new Item[numSlots];

        for (int i = 0; i < tags.Count; i++)
		{
			if (tags[i] == "")
				inventory[i] = null;
			else
				inventory[i] = new Item(ContentLibrary.Instance.Items.Get(tags[i]));
		}
		ContentsWereChanged();
	}
	public override string ComponentId => "container";

	public override List<string> Tags
	{
		get
		{
            if (inventory == null)
            {
                inventory = new Item[numSlots];
            }
			List<string> tags = new List<string>();
			tags.Add(ContainerName);
			tags.Add(numSlots.ToString());
			for(int i = 0; i < numSlots; i++)
			{
				if (inventory[i] != null)
					tags.Add(inventory[i].id);
				else
					tags.Add(string.Empty);
			}
			return tags;
		}
	}
}
