using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InteractableContainer : MonoBehaviour, ISaveable, IInteractableObject 
{
	private const string SavedComponentId = "container";
	private const string ContainerNameTag = "name";
	private const string SlotNumTag = "slots";
	private const string ContentsTag = "contents";
	private const char ContentsTagDelimiter = ',';

	public delegate void ContainerEvent();
	public delegate void DetailedContainerEvent(InteractableContainer container);
	public static ContainerEvent SomeContainerDestroyed;
	public static DetailedContainerEvent ContainerDestroyed;

	private static bool hasSetUpSceneChangeHandler = false;

	[SerializeField] private string containerName;
	[SerializeField] protected int numSlots;
	protected ItemStack[] inventory;

	private static void ResetStaticMembers ()
	{
		hasSetUpSceneChangeHandler = false;
		SomeContainerDestroyed = null;
		ContainerDestroyed = null;
	}

	protected virtual void Start ()
	{
		if (!hasSetUpSceneChangeHandler)
		{
			SceneChangeActivator.OnSceneExit += ResetStaticMembers;
			hasSetUpSceneChangeHandler = true;
		}
	}

	protected virtual void OnDestroy()
	{
		ContainerDestroyed?.Invoke(this);
	}

	public virtual void OnInteract () {}

	public virtual void ContentsWereChanged() {}

	public virtual bool CanHoldItem(ItemStack item)
	{
		return true;
	}

	public virtual bool AttemptAddItem (ItemStack item)
	{
		if (inventory == null)
			inventory = new ItemStack[numSlots];

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

	public virtual bool AttemptPlaceItemInSlot (ItemStack item, int slot, bool ignoreItemAlreadyInSlot = false)
	{
		if (inventory == null)
			inventory = new ItemStack[numSlots];

		if (ignoreItemAlreadyInSlot || inventory[slot] == null)
		{
			inventory[slot] = item;
			ContentsWereChanged();
			return true;
		}
		return false;
	}

	public ItemStack[] GetContainerInventory () {
		if (inventory == null)
		{
			inventory = new ItemStack[numSlots];
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
			foreach (ItemStack item in inventory)
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
	// name = container name, 
	// slots = number of slots,
	// contents = inv contents (comma delimited, blank for no item)

	void ISaveable.SetTags(IDictionary<string, string> tags)
	{
		containerName = tags[ContainerNameTag];
		numSlots = int.Parse(tags[SlotNumTag]);
		string[] contents = tags[ContentsTag].Split(ContentsTagDelimiter);

        inventory = new ItemStack[numSlots];

        for (int i = 0; i < contents.Length; i++)
		{
			if (contents[i] == "")
				inventory[i] = null;
			else
				inventory[i] = new ItemStack(ContentLibrary.Instance.Items.Get(contents[i]));
		}
		ContentsWereChanged();
	}
	string ISaveable.ComponentId => SavedComponentId;

	IDictionary<string, string> ISaveable.GetTags()
	{
        if (inventory == null)
        {
            inventory = new ItemStack[numSlots];
        }
		Dictionary<string, string> tags = new Dictionary<string, string>();
		tags[ContainerNameTag] = containerName;
		tags[SlotNumTag] = numSlots.ToString();
		string contentsTag = "";

		for(int i = 0; i < numSlots; i++)
		{
			if (inventory[i] != null)
				contentsTag += inventory[i].id;
			if (i < numSlots - 1)
				contentsTag += ContentsTagDelimiter;
		}
		tags[ContentsTag] = contentsTag;
		return tags;
	}
}
