using ActorComponents;
using Items;
using UnityEngine;

public static class PickupSystem
{
	public static bool AttemptPickup (Actor actor, IPickuppable itemObject)
	{
		ActorInventory inventory = actor.GetData().Get<ActorInventory>();
		if (inventory == null) return false;
		
		ItemStack item = itemObject.ItemPickup;
		if (item == null) {
			Debug.LogWarning("Tried to pick up null item.");
			return false;
		}

		if (!inventory.AttemptAddItem(item)) return false;
		
		itemObject.OnPickup();
		return true;
	}
}
