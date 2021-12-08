using System;
using System.Collections.Generic;
using GUI.ContainerLayoutElements;
using UnityEngine;

// An object used to brew moonshine
public class Still : MonoBehaviour, ICustomLayoutContainer, ISaveable, IInteractableContainer
{
	private const string FuelSlotLabel = "Fuel";
	private const string IngrSlotLabel = "Ingredient";
	private const string OutSlotLabel = "Output";
	private const string SaveId = "still";
	private const string IngredientSaveId = "ingr";
	private const string FuelSaveId = "fuel";
	private const string OutputSaveId = "out";
	private const char ItemQuantitySeparator = '*';
	
	private const string OutputItem = "moonshine";

	// Percentage points of progress increase per tick when brewing
	private const float ProgressPerTick = 0.002f;

	private readonly List<string> ingredientItemWhitelist = new() { "wheat" };
	private readonly List<string> fuelItemWhitelist = new() { "wood" };
	private readonly List<string> outputItemWhitelist = new(); // Nothing can be placed in the output slot

	[SerializeField] private string containerName = "Still";

	private Action<IContainer> onStateChanged; // Callback for when this object causes its own items to change
	private InventorySlot[] slots;
	private float progress;
	private ulong lastStartTime; // The most recent time, in ticks, the brewing process has started

	string IContainer.Name => containerName;

	int IContainer.SlotCount => 3;

	string ISaveable.ComponentId => SaveId;

	private void Update()
	{
		if (slots == null) return;

		if (slots[1].Contents == null || 
		    slots[0].Contents == null || 
		    (slots[2].Contents != null &&
		     (slots[2].Contents.Id != OutputItem ||
		      slots[2].Contents.IsFull())))
		{
			// Not set up to brew; revert progress to 0.
			progress = 0;
			lastStartTime = TimeKeeper.CurrentTick;
		}
		else
		{
			progress = (TimeKeeper.CurrentTick - lastStartTime) * ProgressPerTick;
			
			if (progress >= 1f)
			{
				slots[0].Contents = slots[0].Contents.Decremented();
				slots[1].Contents = slots[1].Contents.Decremented();

				if (slots[2].Contents == null)
					slots[2].Contents = new ItemStack(OutputItem, 1);
				else
					slots[2].Contents = slots[2].Contents.Incremented();

				// Restart progress
				lastStartTime = TimeKeeper.CurrentTick;
			
				onStateChanged?.Invoke(this);
			}
		}
	}

	public ItemStack Get(int slot)
	{
		if (slots == null) InitializeSlots();
		return slots[slot].Contents;
	}

	/*
	 * Stores the given item in the given slot. Requires that the slot accepts
	 * the given type of item.
	 */
	public void Set(int slot, ItemStack item)
	{
		if (slots == null) InitializeSlots();
		Debug.Assert(item == null || AcceptsItemType(item.Id, slot));
		slots[slot].Contents = item; 
		onStateChanged?.Invoke(this);
	}
	
	public bool AcceptsItemType(string itemId, int slot)
	{
		return slots[slot].CanHoldItem(itemId);
	}
	
	private void InitializeSlots()
	{
		slots = new InventorySlot[3];
		slots[0] = new InventorySlotWhitelisted(ingredientItemWhitelist);
		slots[1] = new InventorySlotWhitelisted(fuelItemWhitelist);
		slots[2] = new InventorySlotWhitelisted(outputItemWhitelist);
		onStateChanged?.Invoke(this);
	}

	void IInteractable.OnInteract()
	{
		// Doesn't do anything upon interacting
	}

	public void SetUpdateListener(Action<IContainer> listener)
	{
		onStateChanged = listener;
	}
	
	List<IContainerLayoutElement> ICustomLayoutContainer.GetLayoutElements()
	{
		return new List<IContainerLayoutElement>
		{
			new ContainerLayoutLabel(() => IngrSlotLabel),
			new ContainerLayoutInvArray(0, 0),
			new ContainerLayoutLabel(() => FuelSlotLabel),
			new ContainerLayoutInvArray(1, 1),
			new ContainerLayoutLabel(() => OutSlotLabel),
			new ContainerLayoutInvArray(2, 2),
			new ContainerLayoutLabel(() => progress.ToString("P1"))
		};
	}

	IDictionary<string, string> ISaveable.GetTags()
	{
		Dictionary<string, string> tags = new();

		if (slots[0].Contents != null)
			tags[IngredientSaveId] = slots[0].Contents.Id + ItemQuantitySeparator + slots[0].Contents.Quantity;
		if (slots[1].Contents != null)
			tags[FuelSaveId] = slots[1].Contents.Id + ItemQuantitySeparator + slots[1].Contents.Quantity;
		if (slots[2].Contents != null)
			tags[OutputSaveId] = slots[2].Contents.Id + ItemQuantitySeparator + slots[2].Contents.Quantity;

		return tags;
	}

	void ISaveable.SetTags(IDictionary<string, string> tags)
	{
		InitializeSlots();
		if (tags.TryGetValue(IngredientSaveId, out string val))
		{
			int quantity = int.Parse(val.Split(ItemQuantitySeparator)[1]);
			slots[0].Contents = new ItemStack(val.Split(ItemQuantitySeparator)[0], quantity);
		}		
		if (tags.TryGetValue(FuelSaveId, out string val2))
		{
			int quantity = int.Parse(val2.Split(ItemQuantitySeparator)[1]);
			slots[1].Contents = new ItemStack(val2.Split(ItemQuantitySeparator)[0], quantity);
		}		
		if (tags.TryGetValue(OutputSaveId, out string val3))
		{
			int quantity = int.Parse(val3.Split(ItemQuantitySeparator)[1]);
			slots[2].Contents = new ItemStack(val3.Split(ItemQuantitySeparator)[0], quantity);
		}
		onStateChanged?.Invoke(this);
	}
}
