using System.Collections.Generic;
using ContentLibraries;
using UnityEngine;


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
	[SerializeField] private bool isEdible = false;
	[SerializeField] private float nutritionalValue = 0.25f;
	[SerializeField] private bool isCraftable = false;
	[SerializeField] private CraftingEnvironment craftingEnv = CraftingEnvironment.Handcrafted;
	[SerializeField] private List<CraftingIngredient> ingredients = null;

	public string DefaultName => itemName;
	public string ItemId => itemId;
	public int MaxStackSize => maxStackSize;
	public string Description => description;
	public Sprite Icon => itemIcon;
	public Category ItemCategory => category;
	public int BaseValue => baseValue;
	public bool IsEdible => isEdible;
	public float NutritionalValue => nutritionalValue;
	public bool IsCraftable => isCraftable;
	public CraftingEnvironment CraftingEnvironment => craftingEnv;
	public List<CraftingIngredient> Ingredients => ingredients;

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

	public virtual string GetItemName(IDictionary<string, string> modifiers)
	{
		if (modifiers.TryGetValue(ItemNameModifier, out string modifiedName))
		{
			return modifiedName;
		}
		return itemName;
	}

	public static ItemData CreateBlank(string id, string name)
	{
		ItemData item = CreateInstance<ItemData>();
		item.itemName = name;
		item.itemId = id;
		return item;
	}

	public static ItemData CreateCorpse(string actorId)
	{
		ItemData item = CreateInstance<ItemData>();
		ActorData actorData = ActorRegistry.Get(actorId).data;
		item.itemName = actorData.ActorName + "'s Corpse";
		item.itemId = "actor:" + actorId;
		item.itemIcon = ContentLibrary.Instance.Races.Get(actorData.RaceID).ItemSprite;
		item.description = "A " + ContentLibrary.Instance.Races.Get(actorData.RaceID).Name + " corpse.";
		return item;
	}
}
