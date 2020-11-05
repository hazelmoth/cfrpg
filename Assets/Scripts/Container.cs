using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

// A collection of extension methods for interacting with IContainer objects
public static class Container
{
	// Returns the number of items that were added successfully.
	public static int AttemptAddItems(this IContainer container, string item, int quantity)
	{
		int added = 0;
		int stackLimit = ContentLibrary.Instance.Items.Get(item).MaxStackSize;

		for (int i = 0; i < container.SlotCount; i++)
		{
			if (added >= quantity) break;

			if (container.GetItem(i) == null)
			{
				if (container.CanHoldItem(item, i))
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

	public static bool AttemptPlaceItemInSlot(this IContainer container, ItemStack item, int slot, bool ignoreItemAlreadyInSlot = false)
	{
		if (item != null && !container.CanHoldItem(item.id, slot)) return false;
		if (ignoreItemAlreadyInSlot || container.GetItem(slot) == null)
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
			if (container.GetItem(i) == null) n++;
		}
		return n;
	}

	public static bool IsEmpty(this IContainer container)
	{
		return (container.GetEmptySlotCount() == container.SlotCount);
	}
}
