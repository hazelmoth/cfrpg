
using UnityEngine;

public class DroppedItemPickupManager
{
	public static bool AttemptPickup (Actor actor, DroppedItem itemObject) {
		ItemData itemData = ContentLibrary.Instance.Items.Get (itemObject.ItemId);
		if (itemData == null)
			return false;
		
		if (actor.GetData().Inventory.AttemptAddItemToInv(new Item(itemData))) {
			GameObject.Destroy (itemObject.gameObject);
			return true;
		}
		return false;
	}
}
