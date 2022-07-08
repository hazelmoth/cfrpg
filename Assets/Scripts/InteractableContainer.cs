using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// A standard interactable item container with saveable state.
public class InteractableContainer : MonoBehaviour, ISaveable, IInteractableContainer
{
	// TODO this should probably derive SaveableContainer
	public const string SavedComponentId = "container";
	private const string ContainerNameTag = "name";
	private const string SlotNumTag = "slots";
	private const string ContentsTag = "contents";
	private const char ContentsTagDelimiter = ',';
	private const char ContentsQuantitySeparator = '*';

	public delegate void DetailedContainerEvent(InteractableContainer container);
	public static DetailedContainerEvent containerDestroyed;

	private static bool hasSetUpSceneChangeHandler = false;

	[SerializeField] private string containerName;
	[SerializeField] protected int numSlots;
	protected InventorySlot[] slots;

	private static void ResetStaticMembers ()
	{
		hasSetUpSceneChangeHandler = false;
		containerDestroyed = null;
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
		containerDestroyed?.Invoke(this);
	}

	public virtual void OnInteract () {}

	protected virtual void ContentsWereChanged() {}

	int IContainer.SlotCount => numSlots;

	string IContainer.Name => containerName;

	ItemStack IContainer.Get(int slot)
	{
		if (slots == null)
		{
			InitializeSlots();
		}
		return slots[slot].Contents;
	}

	void IContainer.Set(int slot, ItemStack item)
	{
		if (slots == null)
		{
			InitializeSlots();
		}
		slots[slot].Contents = item;
		ContentsWereChanged();
	}

	bool IContainer.AcceptsItemType(string itemId, int slot)
	{
		return slots[slot].CanHoldItem(itemId);
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

		InitializeSlots();

        for (int i = 0; i < contents.Length; i++)
		{
			if (contents[i] == "")
				continue;
			else
			{
				string id = contents[i].Split(ContentsQuantitySeparator)[0];
				int quantity = 1;
				if (contents[i].Contains(ContentsQuantitySeparator))
				{
					quantity = Int32.Parse(contents[i].Split(ContentsQuantitySeparator)[1]);
				}
				slots[i].Contents = new ItemStack(id, quantity);
			}
		}
		ContentsWereChanged();
	}

	string ISaveable.ComponentId => SavedComponentId;

	IDictionary<string, string> ISaveable.GetTags()
	{
        if (slots == null)
        {
			InitializeSlots();
        }
		Dictionary<string, string> tags = new Dictionary<string, string>();
		tags[ContainerNameTag] = containerName;
		tags[SlotNumTag] = numSlots.ToString();
		string contentsTag = "";

		for(int i = 0; i < numSlots; i++)
		{
			if (slots[i].Contents != null)
			{
				contentsTag += slots[i].Contents.Id;
				if (slots[i].Contents.Quantity > 1)
				{
					contentsTag += ContentsQuantitySeparator;
					contentsTag += slots[i].Contents.Quantity.ToString();
				}
			}
			if (i < numSlots - 1)
				contentsTag += ContentsTagDelimiter;
		}
		tags[ContentsTag] = contentsTag;
		return tags;
	}

	protected virtual void InitializeSlots()
	{
		slots = new InventorySlot[numSlots];
		for (int i = 0; i < slots.Length; i++)
		{
			slots[i] = new InventorySlot();
		}
	}
}
