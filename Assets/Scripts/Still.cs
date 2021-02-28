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
	private const string SaveId = "still";
	private const string IngredientSaveId = "ingr";
	private const string FuelSaveId = "fuel";
	private const string OutputSaveId = "out";
	private const char ItemQuantitySeperator = '*';

	// Percentage points of progress increase per second when brewing
	private const float ProgressPerTick = 0.002f;

	private readonly List<string> ingredientItemWhitelist = new List<string> { "wheat" };
	private readonly List<string> fuelItemWhitelist = new List<string> { "wood" };
	private readonly List<string> outputItemWhitelist = new List<string>(); // Nothing can be placed in the output slot

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

		if (slots[1].Contents == null || slots[0].Contents == null || slots[2].Contents != null)
		{
			// Not set up to brew; revert progress to 0.
			progress = 0;
			lastStartTime = TimeKeeper.CurrentTick;
		}
		else
		{
			progress = (TimeKeeper.CurrentTick - lastStartTime) * ProgressPerTick;
		}

		if (progress >= 1f)
		{
			slots[0].Contents = null;
			slots[1].Contents = null;
			slots[2].Contents = new ItemStack("flatbread", 1);
			progress = 0;
			
			onStateChanged?.Invoke(this);
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
		Debug.Assert(item == null || AcceptsItemType(item.id, slot));
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
		List<IContainerLayoutElement> elements = new List<IContainerLayoutElement>();
		elements.Add(new ContainerLayoutLabel(() => IngrSlotLabel));
		elements.Add(new ContainerLayoutInvArray(0, 0));
		elements.Add(new ContainerLayoutLabel(() => FuelSlotLabel));
		elements.Add(new ContainerLayoutInvArray(1, 1));
		elements.Add(new ContainerLayoutLabel(() => OutSlotLabel));
		elements.Add(new ContainerLayoutInvArray(2, 2));
		elements.Add(new ContainerLayoutLabel(() => progress.ToString("P1")));
		return elements;
	}

	IDictionary<string, string> ISaveable.GetTags()
	{
		Dictionary<string, string> tags = new Dictionary<string, string>();

		if (slots[0].Contents != null)
			tags[IngredientSaveId] = slots[0].Contents.id + ItemQuantitySeperator + slots[0].Contents.quantity;
		if (slots[1].Contents != null)
			tags[FuelSaveId] = slots[1].Contents.id + ItemQuantitySeperator + slots[1].Contents.quantity;
		if (slots[2].Contents != null)
			tags[OutputSaveId] = slots[2].Contents.id + ItemQuantitySeperator + slots[2].Contents.quantity;

		return tags;
	}

	void ISaveable.SetTags(IDictionary<string, string> tags)
	{
		InitializeSlots();
		if (tags.TryGetValue(IngredientSaveId, out string val))
		{
			int quantity = Int32.Parse(val.Split(ItemQuantitySeperator)[1]);
			slots[0].Contents = new ItemStack(val.Split(ItemQuantitySeperator)[0], quantity);
		}		
		if (tags.TryGetValue(FuelSaveId, out string val2))
		{
			int quantity = Int32.Parse(val2.Split(ItemQuantitySeperator)[1]);
			slots[1].Contents = new ItemStack(val2.Split(ItemQuantitySeperator)[0], quantity);
		}		
		if (tags.TryGetValue(OutputSaveId, out string val3))
		{
			int quantity = Int32.Parse(val3.Split(ItemQuantitySeperator)[1]);
			slots[2].Contents = new ItemStack(val3.Split(ItemQuantitySeperator)[0], quantity);
		}
		onStateChanged?.Invoke(this);
	}
}