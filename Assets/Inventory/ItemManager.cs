using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Stores the master list of items.
public class ItemManager : MonoBehaviour {

	[SerializeField] List<Item> itemList;
	public static ItemManager instance;

	// Use this for initialization
	void Start () {
		instance = this;
	}

	public Item GetItemByIndex (int index) {
		return itemList [index];
	}

	public Item GetItemById (string id) {
		foreach (Item item in itemList) {
			if (item.GetItemId() == id) {
				return item;
			}
		}
		return null;
	}

	public Item GetItemByName (string name) {
		foreach (Item item in itemList) {
			if (item.GetItemName() == name) {
				return item;
			}
		}
		return null;
	}
}
