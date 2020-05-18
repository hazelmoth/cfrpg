using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Stores the master list of items.
public class ItemLibrary {
	private const string LIBRARY_ASSET_PATH = "ItemLibrary";

	private IDictionary<string, ItemData> items;
	private IDictionary<ItemData.Category, ISet<ItemData>> categories;

	public void LoadLibrary ()
	{
		ItemLibraryAsset loadedLibraryAsset = (ItemLibraryAsset)(Resources.Load(LIBRARY_ASSET_PATH, typeof(ScriptableObject)));

        items = new Dictionary<string, ItemData>();
        categories = new Dictionary<ItemData.Category, ISet<ItemData>>();

        foreach (ItemData item in loadedLibraryAsset.items)
		{
			items.Add(item.ItemId, item);

			if (!categories.ContainsKey(item.ItemCategory))
			{
				categories.Add(item.ItemCategory, new HashSet<ItemData>());
			}
			categories[item.ItemCategory].Add(item);
        }
	}

	public ISet<ItemData> GetByCategory(ItemData.Category category)
	{
		if (categories.ContainsKey(category))
		{
			return categories[category];
		}

		return new HashSet<ItemData>();
	}

	public ItemData Get (string id)
	{
		if (items.ContainsKey(id))
		{
			return items[id];
		}
		return null;
	}

    public List<Hat> GetHats ()
    {
        List<Hat> hatList = new List<Hat>();
        foreach (ItemData item in items.Values) {
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
        foreach (ItemData item in items.Values)
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
        foreach (ItemData item in items.Values)
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
