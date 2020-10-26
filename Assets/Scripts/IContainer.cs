using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IContainer
{
	string Name { get; }
	int SlotCount { get; }

	InventorySlot[] Slots { get; }
	ItemStack GetItem(int slot);
	void SetItem(int slot, ItemStack item);
}
