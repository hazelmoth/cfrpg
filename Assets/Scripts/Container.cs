using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A collection of extension methods for interacting with IContainer objects
public static class Container
{
	// Returns the number of items that were added successfully.
	public static int AttemptAddItems(this IContainer container, string item, int quantity)
	{
		InventorySlot[] slots = container.Slots;

		int added = 0;
		int stackLimit = ContentLibrary.Instance.Items.Get(item).MaxStackSize;

		for (int i = 0; i < container.SlotCount; i++)
		{
			if (added >= quantity) break;

			if (container.GetItem(i) == null)
			{
				if (slots[i].CanHoldItem(item))
				{
					container.SetItem(i, new ItemStack(item, 0));
				}
			}
			if (container.GetItem(i)?.id == item)
			{
				int stackSize = Math.Min(quantity - added, stackLimit - container.GetItem(i).quantity);
				stackSize = stackSize < 0 ? 0 : stackSize;
				container.GetItem(i).quantity += stackSize;
				added += stackSize;
			}
		}
		return added;
	}

	public static bool CanHoldItem(this IContainer container, int slot, string item)
	{
		InventorySlot invSlot = container.Slots[slot];
		return invSlot.CanHoldItem(item);
	}

	public static bool AttemptPlaceItemInSlot(this IContainer container, ItemStack item, int slot, bool ignoreItemAlreadyInSlot = false)
	{
		if (item != null && !container.CanHoldItem(slot, item.id)) return false;
		if (ignoreItemAlreadyInSlot || container.Slots[slot].Contents == null)
		{
			container.SetItem(slot, item);
			return true;
		}
		return false;
	}

	public static int GetEmptySlotCount(this IContainer container)
	{
		int n = 0;
		for (int i = 0; i < container.SlotCount; i++)
		{
			if (container.GetItem(n) == null) n++;
		}
		return n;
	}
}
