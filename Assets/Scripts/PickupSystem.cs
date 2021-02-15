using UnityEngine;

public static class PickupSystem
{
	public static bool AttemptPickup (Actor actor, IPickuppable itemObject) 
	{
		ItemStack item = itemObject.ItemPickup;
		Debug.Log("Attempting pickup:");
		if (item == null) {
			Debug.LogWarning("Tried to pick up null item.");
			return false;
		}
		
		if (actor.GetData().Inventory.AttemptAddItem(item)) {
			Debug.Log("pickup succeeded.");
			GameObject.Destroy (((MonoBehaviour)itemObject).gameObject);
			return true;
		}
		Debug.Log("item pickup failed.");
		return false;
	}
}
