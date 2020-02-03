using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Stores the master list of items.
public class ItemLibrary : MonoBehaviour {

	[SerializeField] List<Item> itemList = new List<Item>();
	public static ItemLibrary instance;

	// Use this for initialization
	void Start () {
		instance = this;
	}

	public void LoadLibrary ()
	{
		// TODO: Automatically generated library object instead of doing it manually
	}

	public static Item GetItemByIndex (int index) {
		return instance.itemList [index];
	}

	public static Item GetItemById (string id) {
		foreach (Item item in instance.itemList) {
			if (item.GetItemId() == id) {
				return item;
			}
		}
		return null;
	}

    public static List<Hat> GetHats ()
    {
        List<Hat> hatList = new List<Hat>();
        foreach (Item item in instance.itemList) {
            Hat hat = item as Hat;
            if (hat != null)
            {
                hatList.Add(hat);
            }
        }
        return hatList;
    }

    public static List<Shirt> GetShirts()
    {
        List<Shirt> shirtList = new List<Shirt>();
        foreach (Item item in instance.itemList)
        {
            Shirt shirt = item as Shirt;
            if (shirt != null)
            {
                shirtList.Add(shirt);
            }
        }
        return shirtList;
    }
    public static List<Pants> GetPants()
    {
        List<Pants> pantsList = new List<Pants>();
        foreach (Item item in instance.itemList)
        {
            Pants pants = item as Pants;
            if (pants != null)
            {
                pantsList.Add(pants);
            }
        }
        return pantsList;
    }
}
