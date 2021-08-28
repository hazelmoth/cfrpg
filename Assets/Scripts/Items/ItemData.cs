using System.Collections.Generic;
using System.Collections.Immutable;
using UnityEngine;

namespace Items
{
	[CreateAssetMenu(fileName="NewItem", menuName = "Items/Base Item", order = 1)]
	public class ItemData : ScriptableObject
	{
		private const string ItemNameModifier = "name";

		[SerializeField] private string itemName = null;
		[SerializeField] private string itemId = null;
		[SerializeField] private int maxStackSize = 1;
		[SerializeField] private string description = null;
		[SerializeField] private Sprite itemIcon = null;
		[SerializeField] private Category category = Category.Misc;
		[SerializeField] private int baseValue = 20;
		[SerializeField] private bool isCraftable = false;
		[SerializeField] private CraftingEnvironment craftingEnv = CraftingEnvironment.Handcrafted;
		[SerializeField] private List<CraftingIngredient> ingredients = null;

		public string DefaultName => itemName;
		public Sprite DefaultIcon => itemIcon;
		public string ItemId => itemId;
		public int MaxStackSize => maxStackSize;
		public string Description => description;

		public Category ItemCategory => category;
		public int BaseValue => baseValue;
		public bool IsCraftable => isCraftable;
		public CraftingEnvironment CraftingEnvironment => craftingEnv;
		public ImmutableList<CraftingIngredient> Ingredients => ingredients.ToImmutableList();

		public enum Category
		{
			Weapons,
			Tools,
			Clothing,
			Food,
			Drugs,
			Misc
		}

		[System.Serializable]
		public class CraftingIngredient
		{
			public string itemId;
			public int count;
		}

		public string GetItemName(string fullItemId) => GetItemName(ItemIdParser.ParseModifiers(fullItemId));
		public virtual string GetItemName(IDictionary<string, string> modifiers)
		{
			if (modifiers.TryGetValue(ItemNameModifier, out string modifiedName))
			{
				return modifiedName;
			}
			return itemName;
		}

		public Sprite GetIcon(string fullItemId) => GetIcon(ItemIdParser.ParseModifiers(fullItemId));
		public virtual Sprite GetIcon(IDictionary<string, string> modifiers) => itemIcon;

		public static ItemData CreateBlank(string id, string name)
		{
			ItemData item = CreateInstance<ItemData>();
			item.itemName = name;
			item.itemId = id;
			return item;
		}
	}
}
