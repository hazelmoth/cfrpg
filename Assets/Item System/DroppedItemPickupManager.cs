using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedItemPickupManager : MonoBehaviour
{
	public static bool AttemptPickup (Actor actor, DroppedItem itemObject) {
		Item item = ItemLibrary.GetItemById (itemObject.ItemId);
		if (item == null)
			return false;
		
		if (actor.Inventory.AttemptAddItemToInv(item)) {
			GameObject.Destroy (itemObject.gameObject);
			return true;
		}
		return false;
	}
}
