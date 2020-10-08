using System.Collections.Generic;
using UnityEngine;

public class WoodPile : InteractableContainer
{
	[SerializeField] private SpriteRenderer spriteRenderer;
	[SerializeField] private List<Sprite> woodSprites;

	private const string logItemId = "wood";

	// Only accept wood items
	private List<string> itemWhitelist = new List<string> { "wood" };


	protected override void Start()
    {
		base.Start();
		if (inventory == null)
		{
			inventory = new ItemStack[numSlots];
			AttemptAddItem(new ItemStack(ContentLibrary.Instance.Items.Get(logItemId)));
		}
		UpdateWoodSprites();
    }

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	public override void OnInteract()
	{
		base.OnInteract();
		UpdateWoodSprites();
	}
	public override void ContentsWereChanged()
	{
		// Destroy this wood pile if contains no wood
		if (NumFullSlots == 0)
		{
			WorldMapManager.RemoveEntityAtPoint(transform.position.ToVector2Int(), SceneObjectManager.GetSceneIdForObject(gameObject));
		}

		base.ContentsWereChanged();
		UpdateWoodSprites();
	}
	public override bool CanHoldItem(ItemStack item)
	{
		if (item != null && !ItemIsInWhitelist(item.GetData()))
			return false;

		return base.CanHoldItem(item);
	}

	public override bool AttemptAddItem(ItemStack item)
	{
		if (!CanHoldItem(item))
			return false;

		return base.AttemptAddItem(item);
	}
	public override bool AttemptPlaceItemInSlot(ItemStack item, int slot, bool ignoreItemAlreadyInSlot = false)
	{
		if (!CanHoldItem(item))
			return false;

		return base.AttemptPlaceItemInSlot(item, slot, ignoreItemAlreadyInSlot);
	}

	private bool ItemIsInWhitelist (ItemData item)
	{
		if (item == null)
			return true;

		foreach (string itemId in itemWhitelist)
		{
			if (item.ItemId == itemId)
				return true;
		}
		return false;
	}

	private void UpdateWoodSprites()
	{
		// Set the wood sprite based on how full the container is
		float fullness = (float)NumFullSlots / numSlots;
		int spriteNum = Mathf.FloorToInt((woodSprites.Count - 1) * fullness);
		spriteRenderer.sprite = woodSprites[spriteNum];
	}
}
