using System;
using System.Collections;
using System.Collections.Generic;
using ContentLibraries;
using UnityEngine;
using UnityEngine.Assertions.Must;

/// A collection of extension methods for interacting with IContainer objects
public static class Container
{
	/// Adds the given quantity of the specified item to the container, if there are slots
	/// available that are willing to accept the item. Adds one item at a time and returns
	/// the number of items that were added successfully.
	public static int AttemptAdd(this IContainer container, string item, int quantity)
	{
		int added = 0;
		int stackLimit = ContentLibrary.Instance.Items.Get(item).MaxStackSize;

		for (int i = 0; i < container.SlotCount; i++)
		{
			if (added >= quantity) break;

			if (container.Get(i) == null)
			{
				if (container.AcceptsItemType(item, i))
				{
					container.Set(i, new ItemStack(item, 0));
				}
			}
			if (container.Get(i)?.Id == item)
			{
				int stackSize = Math.Min(quantity - added, stackLimit - container.Get(i).Quantity);
				stackSize = stackSize < 0 ? 0 : stackSize;
				container.Set(i, new ItemStack(container.Get(i).Id, container.Get(i).Quantity + stackSize));
				added += stackSize;
			}
		}
		return added;
	}

	public static int GetEmptySlotCount(this IContainer container)
	{
		int n = 0;
		for (int i = 0; i < container.SlotCount; i++)
		{
			if (container.Get(i) == null) n++;
		}
		return n;
	}

	public static bool IsEmpty(this IContainer container)
	{
		return (container.GetEmptySlotCount() == container.SlotCount);
	}

	public static void Clear(this IContainer container)
	{
		for (int i = 0; i < container.SlotCount; i++)
		{
			container.Set(i, null);
		}
	}

	/// Attempts to remove the given quantity of the specified item from the container, if
	/// there are enough items in the container to remove. Returns the number of items
	/// that were removed successfully.
	public static int AttemptRemove(this IContainer container, string item, int quantity)
	{
		int removed = 0;
		int stackLimit = ContentLibrary.Instance.Items.Get(item).MaxStackSize;

		for (int i = 0; i < container.SlotCount; i++)
		{
			if (removed >= quantity) break;
			if (container.Get(i)?.Id != item) continue;

			int removeFromStack = Math.Min(quantity - removed, container.Get(i).Quantity);
			removeFromStack = removeFromStack < 0 ? 0 : removeFromStack;
			container.Set(i, container.Get(i).AddQuantity(-removeFromStack));
			removed += removeFromStack;
		}
		return removed;
	}
}
