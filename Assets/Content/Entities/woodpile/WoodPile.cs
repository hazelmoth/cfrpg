using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodPile : InteractableContainer
{
	[SerializeField] SpriteRenderer spriteRenderer;
	[SerializeField] List<Sprite> woodSprites;

	const string logItemId = "log";

	// Only accept wood items
	List<string> itemWhitelist = new List<string> { "log" };


    void Start()
    {
		if (inventory == null)
		{
			inventory = new Item[numSlots];
		}
		AttemptAddItem(ItemLibrary.GetItemById(logItemId));
		UpdateWoodSprites();
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
	public override bool CanHoldItem(Item item)
	{
		if (!ItemIsInWhitelist(item))
			return false;

		return base.CanHoldItem(item);
	}

	public override bool AttemptAddItem(Item item)
	{
		if (!ItemIsInWhitelist(item))
			return false;

		return base.AttemptAddItem(item);
	}
	public override bool AttemptPlaceItemInSlot(Item item, int slot, bool ignoreItemAlreadyInSlot = false)
	{
		if (!ItemIsInWhitelist(item))
			return false;

		return base.AttemptPlaceItemInSlot(item, slot, ignoreItemAlreadyInSlot);
	}

	bool ItemIsInWhitelist (Item item)
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

	void UpdateWoodSprites()
	{
		// Set the wood sprite based on how full the container is
		float fullness = (float)NumFullSlots / numSlots;
		int spriteNum = Mathf.FloorToInt((woodSprites.Count - 1) * fullness);
		spriteRenderer.sprite = woodSprites[spriteNum];
	}
}
