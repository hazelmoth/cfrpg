using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// An object used to brew moonshine
public class Still : MonoBehaviour, ICustomLayoutContainer, IInteractable, ISaveable
{
	private const string FuelSlotLabel = "Fuel";
	private const string IngrSlotLabel = "Ingredient";
	private const string OutSlotLabel = "Output";
	private const string saveId = "still";
	private const string ingredientSaveId = "ingr";
	private const string fuelSaveId = "fuel";
	private const string outputSaveId = "out";
	private const char itemQuantitySeperator = '*';

	private readonly List<string> ingredientItemWhitelist = new List<string> { "wheat" };
	private readonly List<string> fuelItemWhitelist = new List<string> { "wood" };
	private readonly List<string> outputItemWhitelist = new List<string>(); // Nothing can be placed in the output slot

	[SerializeField] private string containerName = "Still";

	private InventorySlot[] slots;

	string IContainer.Name => containerName;

	int IContainer.SlotCount => 3;

	string ISaveable.ComponentId => saveId;


	ItemStack IContainer.Get(int slot)
	{
		if (slots == null) InitializeSlots();
		return slots[slot].Contents;
	}

	List<IContainerLayoutElement> ICustomLayoutContainer.GetLayoutElements()
	{
		List<IContainerLayoutElement> elements = new List<IContainerLayoutElement>();
		elements.Add(new ContainerLayoutLabel(IngrSlotLabel));
		elements.Add(new ContainerLayoutInvArray(0, 0));
		elements.Add(new ContainerLayoutLabel(FuelSlotLabel));
		elements.Add(new ContainerLayoutInvArray(1, 1));
		elements.Add(new ContainerLayoutLabel(OutSlotLabel));
		elements.Add(new ContainerLayoutInvArray(2, 2));
		return elements;
	}

	IDictionary<string, string> ISaveable.GetTags()
	{
		Dictionary<string, string> tags = new Dictionary<string, string>();

		if (slots[0].Contents != null)
			tags[ingredientSaveId] = slots[0].Contents.id + itemQuantitySeperator + slots[0].Contents.quantity;
		if (slots[1].Contents != null)
			tags[fuelSaveId] = slots[1].Contents.id + itemQuantitySeperator + slots[1].Contents.quantity;
		if (slots[2].Contents != null)
			tags[outputSaveId] = slots[2].Contents.id + itemQuantitySeperator + slots[2].Contents.quantity;

		return tags;
	}

	void IInteractable.OnInteract()
	{
		
	}

	void IContainer.Set(int slot, ItemStack item)
	{
		if (slots == null) InitializeSlots();
		slots[slot].Contents = item; 
	}

	void ISaveable.SetTags(IDictionary<string, string> tags)
	{
		InitializeSlots();
		if (tags.TryGetValue(ingredientSaveId, out string val))
		{
			int quantity = Int32.Parse(val.Split(itemQuantitySeperator)[1]);
			slots[0].Contents = new ItemStack(val.Split(itemQuantitySeperator)[0], quantity);
		}		
		if (tags.TryGetValue(fuelSaveId, out string val2))
		{
			int quantity = Int32.Parse(val.Split(itemQuantitySeperator)[1]);
			slots[1].Contents = new ItemStack(val.Split(itemQuantitySeperator)[0], quantity);
		}		
		if (tags.TryGetValue(outputSaveId, out string val3))
		{
			int quantity = Int32.Parse(val.Split(itemQuantitySeperator)[1]);
			slots[2].Contents = new ItemStack(val.Split(itemQuantitySeperator)[0], quantity);
		}
	}

	private void InitializeSlots()
	{
		slots = new InventorySlot[3];
		slots[0] = new InventorySlotWhitelisted(ingredientItemWhitelist);
		slots[1] = new InventorySlotWhitelisted(fuelItemWhitelist);
		slots[2] = new InventorySlotWhitelisted(outputItemWhitelist);
	}

	bool IContainer.CanHoldItem(string itemId, int slot)
	{
		return slots[slot].CanHoldItem(itemId);
	}
}
