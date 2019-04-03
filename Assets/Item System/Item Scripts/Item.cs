using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName="NewItem", menuName = "Items/Base Item", order = 1)]
public class Item : ScriptableObject {
	

	[SerializeField] public string itemName;
	[SerializeField] public string itemId;
	[SerializeField] public Sprite itemIcon;


	public string GetItemName() {
		return itemName;
	}

	public string GetItemId() {
		return itemId;
	}

	public Sprite getIconSprite() {
		return itemIcon;
	}
}
