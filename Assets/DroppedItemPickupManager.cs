using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedItemPickupManager : MonoBehaviour
{
	public static bool AttemptPickup (DroppedItem itemObject) {
		Item item = ItemManager.GetItemById (itemObject.ItemId);
		if (item == null)
			return false;
		
		if (PlayerInventory.AttemptAddItemToInv(item)) {
			GameObject.Destroy (itemObject.gameObject);
			return true;
		}
		return false;
	}
}
