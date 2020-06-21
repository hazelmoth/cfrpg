using Items;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public ItemData Get(string id)
    {
        if (id != null && items.ContainsKey(id))
        {
            return items[id];
        }
        return ItemData.CreateBlank(id, "\"" + id + "\" (MISSING)");
	}

    public List<ItemData> GetAll()
    {
        return items.Values.ToList();
    }

    public List<ItemData> GetHats ()
    {
        List<ItemData> hatList = new List<ItemData>();
        foreach (ItemData item in items.Values) {
            IHat hat = item as IHat;
            if (hat != null)
            {
                hatList.Add(item);
            }
        }
        return hatList;
    }

    public List<ItemData> GetShirts()
    {
        List<ItemData> shirtList = new List<ItemData>();
        foreach (ItemData item in items.Values)
        {
            if (item as Shirt != null)
            {
                shirtList.Add(item);
            }
        }
        return shirtList;
    }
    public List<ItemData> GetPants()
    {
        List<ItemData> pantsList = new List<ItemData>();
        foreach (ItemData item in items.Values)
        {
            if (item as Pants != null)
            {
                pantsList.Add(item);
            }
        }
        return pantsList;
    }
}
