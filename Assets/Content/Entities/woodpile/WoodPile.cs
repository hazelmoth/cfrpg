using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodPile : InteractableContainer
{
	[SerializeField] SpriteRenderer spriteRenderer;
	[SerializeField] List<Sprite> woodSprites;

	// Only accept wood items
	List<string> itemWhitelist = new List<string> { "log" };


    void Start()
    {
		if (inventory == null)
		{
			inventory = new Item[numSlots];
		}
		UpdateWoodSprites();
    }

	public override void OnInteract()
	{
		base.OnInteract();
		UpdateWoodSprites();
	}
	public override void ContentsWereChanged()
	{
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
		foreach (string itemId in itemWhitelist)
		{
			if (item.ItemId == itemId)
				return true;
		}
		return false;
	}

	void UpdateWoodSprites()
	{
		int contentsAmount = 0;
		foreach (Item item in inventory)
		{
			if (item != null)
				contentsAmount++;
		}
		// Set the wood sprite based on how full the container is
		float fullness = (float)contentsAmount / numSlots;
		int spriteNum = Mathf.FloorToInt((woodSprites.Count - 1) * fullness);
		spriteRenderer.sprite = woodSprites[spriteNum];
	}
}
