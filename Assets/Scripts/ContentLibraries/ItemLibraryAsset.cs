using System.Collections.Generic;
using Items;
using UnityEngine;
using ItemData = Items.ItemData;

namespace ContentLibraries
{
	public class ItemLibraryAsset : ScriptableObject
	{
		public List<ItemData> items;
	}
}
