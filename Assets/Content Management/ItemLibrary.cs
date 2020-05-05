using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Stores the master list of items.
public class ItemLibrary {
	private const string LIBRARY_ASSET_PATH = "ItemLibrary";

	private List<Item> itemList;

	public void LoadLibrary ()
	{
		ItemLibraryAsset loadedLibraryAsset = (ItemLibraryAsset)(Resources.Load(LIBRARY_ASSET_PATH, typeof(ScriptableObject)));

		itemList = loadedLibraryAsset.items;
	}

	public Item GetItemById (string id)
	{
		foreach (Item item in itemList) {
			if (item.GetItemId() == id) {
				return item;
			}
		}
		return null;
	}

    public List<Hat> GetHats ()
    {
        List<Hat> hatList = new List<Hat>();
        foreach (Item item in itemList) {
            Hat hat = item as Hat;
            if (hat != null)
            {
                hatList.Add(hat);
            }
        }
        return hatList;
    }

    public List<Shirt> GetShirts()
    {
        List<Shirt> shirtList = new List<Shirt>();
        foreach (Item item in itemList)
        {
            Shirt shirt = item as Shirt;
            if (shirt != null)
            {
                shirtList.Add(shirt);
            }
        }
        return shirtList;
    }
    public List<Pants> GetPants()
    {
        List<Pants> pantsList = new List<Pants>();
        foreach (Item item in itemList)
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
