using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavedDroppedItem
{
	public Vector2Serializable location;
	public string scene;
	public ItemStack item;

	public SavedDroppedItem(Vector2Serializable location, string scene, ItemStack item)
	{
		this.location = location;
		this.scene = scene;
		this.item = item;
	}
}
