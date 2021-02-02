using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IContainer
{
	string Name { get; }

	int SlotCount { get; }

	ItemStack Get(int slot);

	void Set(int slot, ItemStack item);

	bool CanHoldItem(string itemId, int slot);
}
