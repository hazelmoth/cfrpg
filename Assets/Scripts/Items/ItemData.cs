using System.Collections.Generic;
using System.Collections.Immutable;
using Crafting;
using UnityEngine;

namespace Items
{
	[CreateAssetMenu(fileName="NewItem", menuName = "Items/Base Item", order = -1)]
	public class ItemData : ScriptableObject
	{
		public const string ItemNameModifier = "name";

		[SerializeField] private string itemName = null;
		[SerializeField] private string itemId = null;
		[SerializeField] private int maxStackSize = 1;
		[SerializeField] private string description = null;
		[SerializeField] private Sprite itemIcon = null;
		[SerializeField] private Category category = Category.Misc;
		[SerializeField] private int baseValue = 20;

		public string DefaultName => itemName;
		public Sprite DefaultIcon => itemIcon;
		public string ItemId => itemId;
		public int MaxStackSize => maxStackSize;
		public string Description => description;

		public Category ItemCategory => category;
		public int BaseValue => baseValue;

		public enum Category
		{
			Weapons,
			Tools,
			Clothing,
			Food,
			Drugs,
			Misc
		}

		/// Returns the name of a particular instance of this item, based on its modifiers.
		public string GetItemName(string fullItemId) => GetItemName(ItemIdParser.ParseModifiers(fullItemId));

		/// Returns the name of a particular instance of this item, based on its modifiers.
		public virtual string GetItemName(IDictionary<string, string> modifiers)
		{
			if (modifiers.TryGetValue(ItemNameModifier, out string modifiedName))
			{
				return modifiedName;
			}
			return itemName;
		}

		/// Returns the icon of a particular instance of this item, based on its modifiers.
		public Sprite GetIcon(string fullItemId) => GetIcon(ItemIdParser.ParseModifiers(fullItemId));

		/// Returns the icon of a particular instance of this item, based on its modifiers.
		public virtual Sprite GetIcon(IDictionary<string, string> modifiers) => itemIcon;

		/// Returns a blank item with the given name and id.
		public static ItemData CreateBlank(string id, string name)
		{
			ItemData item = CreateInstance<ItemData>();
			item.itemName = name;
			item.itemId = id;
			return item;
		}
	}
}
