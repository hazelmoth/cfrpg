using System;
using System.Collections.Generic;
using UnityEngine;

/// A basic container whose contents are saveable. Not a MonoBehaviour; intended to be
/// used compositionally.
public class SaveableContainerData : IContainer
{
	private const string ContentsTag = "contents";
	private const char ContentsTagDelimiter = ',';
	private const char ContentsQuantitySeparator = '*';

	/// The actual contents of the container.
	/// Each InventorySlot may or may not be empty.
	private readonly InventorySlot[] slots;

	public SaveableContainerData(InventorySlot[] slots)
	{
		Debug.Assert(slots != null, "Slots cannot be null.");
		this.slots = slots;
	}

	public string Name => "SaveableContainerData";

	/// The number of slots in the container.
	public int SlotCount => slots.Length;

	public ItemStack Get(int slot)
	{
		return slots[slot].Contents;
	}

	public void Set(int slot, ItemStack item)
	{
		slots[slot].Contents = item;
	}

	public bool AcceptsItemType(string itemId, int slot)
	{
		return slots[slot].CanHoldItem(itemId);
	}


	// SAVE TAGS (1)
	// contents = inv contents (comma delimited, blank for no item)

	public void SetTags(IDictionary<string, string> tags)
	{
		string[] contents = tags[ContentsTag].Split(ContentsTagDelimiter);

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

	public IDictionary<string, string> GetTags()
	{
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
