using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName="NewItem", menuName = "Items/Base Item", order = 1)]
public class ItemData : ScriptableObject {
	

	[SerializeField] private string itemName = null;
	[SerializeField] private string itemId = null;
	[SerializeField] private string description = null;
	[SerializeField] private Sprite itemIcon = null;
	[SerializeField] private Category category = Category.Misc;
	[SerializeField] private bool isEdible = false;
	[SerializeField] private float nutritionalValue = 0.25f;
	[SerializeField] private bool isCraftable = false;
	[SerializeField] private CraftingReq craftingReq = CraftingReq.None;
	[SerializeField] private List<CraftingIngredient> ingredients = null;

	public string ItemName => itemName;
	public string ItemId => itemId;
	public string Description => description;
	public Sprite ItemIcon => itemIcon;
	public Category ItemCategory => category;
	public bool IsEdible => isEdible;
	public float NutritionalValue => nutritionalValue;
	public bool IsCraftable => isCraftable;
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
}
