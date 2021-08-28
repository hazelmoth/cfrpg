using Items;
using UnityEngine;

public static class PickupSystem
{
	public static bool AttemptPickup (Actor actor, IPickuppable itemObject) 
	{
		ItemStack item = itemObject.ItemPickup;
		if (item == null) {
			Debug.LogWarning("Tried to pick up null item.");
			return false;
		}
		
		if (actor.GetData().Inventory.AttemptAddItem(item)) {
			itemObject.OnPickup();
			return true;
		}
		// Inventory is full.
		return false;
	}
}
