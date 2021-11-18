using System;
using System.Collections.Generic;
using UnityEngine;

/// A base class for containers whose contents are saveable.
public abstract class SaveableContainer : MonoBehaviour, IContainer, ISaveable
{
	private const string ContentsTag = "contents";
	private const char ContentsTagDelimiter = ',';
	private const char ContentsQuantitySeparator = '*';

	private int numSlots;

	/// The actual contents of the container.
	/// Each InventorySlot may or may not be empty.
	protected InventorySlot[] slots;

	/// The number of slots in the container.
	public abstract int SlotCount { get; }

	/// The ID to be used for this class when storing its save data.
	protected abstract string SavedComponentId { get; }

	/// Initializes the contents of the slots array.
	/// Override this to define slots with specific properties.
	protected virtual void InitializeSlots()
	{
		slots = new InventorySlot[SlotCount];
		for (int i = 0; i < slots.Length; i++)
		{
			slots[i] = new InventorySlot();
		}
	}

	public abstract string Name { get; }

	public virtual ItemStack Get(int slot)
	{
		if (slots == null)
		{
			InitializeSlots();
		}
		return slots[slot].Contents;
	}

	public virtual void Set(int slot, ItemStack item)
	{
		if (slots == null)
		{
			InitializeSlots();
		}
		slots[slot].Contents = item;
	}

	public bool AcceptsItemType(string itemId, int slot)
	{
		return slots[slot].CanHoldItem(itemId);
	}

	public string ComponentId => SavedComponentId;

	// SAVE TAGS
	// contents = inv contents (comma delimited, blank for no item)

	public virtual void SetTags(IDictionary<string, string> tags)
	{
		string[] contents = tags[ContentsTag].Split(ContentsTagDelimiter);

		InitializeSlots();

        for (int i = 0; i < contents.Length; i++)
        {
	        if (contents[i] == "") continue;

	        string id = contents[i].Split(ContentsQuantitySeparator)[0];
	        int quantity = 1;
	        if (contents[i].Contains(ContentsQuantitySeparator))
	        {
		        quantity = Int32.Parse(contents[i].Split(ContentsQuantitySeparator)[1]);
	        }
	        slots[i].Contents = new ItemStack(id, quantity);
        }
	}

	public virtual IDictionary<string, string> GetTags()
	{
        if (slots == null)
        {
			InitializeSlots();
        }
		Dictionary<string, string> tags = new();
		string contentsTag = "";

		for(int i = 0; i < SlotCount; i++)
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
			if (i < SlotCount - 1)
				contentsTag += ContentsTagDelimiter;
		}
		tags[ContentsTag] = contentsTag;
		return tags;
	}
}
