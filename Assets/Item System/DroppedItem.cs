using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Describes a physical gameobject for a dropped item
public class DroppedItem : MonoBehaviour
{
	[SerializeField] SpriteRenderer spriteRenderer = null;
	string itemId;

	public string ItemId {get{return itemId;}}

	public void SetItem (string itemId) {
		this.itemId = itemId;
		if (ItemManager.GetItemById (itemId) != null)
			spriteRenderer.sprite = ItemManager.GetItemById (itemId).itemIcon;
		else {
			Debug.LogError ("someone tried to set a dropped item to a nonexistent item id");
		}
	}

}
