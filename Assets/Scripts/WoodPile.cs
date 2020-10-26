using System.Collections.Generic;
using UnityEngine;

public class WoodPile : InteractableContainer
{
	[SerializeField] private SpriteRenderer spriteRenderer = null;
	[SerializeField] private List<Sprite> woodSprites = null;

	private const string logItemId = "wood";

	// Only accept wood items
	private List<string> itemWhitelist = new List<string> { logItemId };


	protected override void Start()
    {
		base.Start();
		if (slots == null)
		{
			InitializeSlots();
			this.AttemptAddItems(logItemId, 1); // Every woodpile starts with a single piece of wood :)
		}
		UpdateWoodSprites();
    }


	public bool IsFull
	{
		get
		{
			foreach (InventorySlot slot in slots)
			{
				if (slot.Contents == null || slot.Contents.quantity < ContentLibrary.Instance.Items.Get(slot.Contents.id).MaxStackSize)
				{
					return false;
				}
			}
			return true;
		}
	}

	public override void OnInteract()
	{
		base.OnInteract();
		UpdateWoodSprites();
	}
	protected override void ContentsWereChanged()
	{
		// Destroy this wood pile if contains no wood
		if (this.GetEmptySlotCount() == numSlots)
		{
			WorldMapManager.RemoveEntityAtPoint(transform.position.ToVector2Int(), SceneObjectManager.GetSceneIdForObject(gameObject));
		}

		base.ContentsWereChanged();
		UpdateWoodSprites();
	}

	protected override void InitializeSlots()
	{
		slots = new InventorySlotWhitelisted[numSlots];
		for (int i = 0; i < numSlots; i++)
		{
			slots[i] = new InventorySlotWhitelisted(itemWhitelist);
		}
	}

	private void UpdateWoodSprites()
	{
		// Set the wood sprite based on how full the container is
		float fullness = (float)(numSlots - this.GetEmptySlotCount()) / numSlots;
		int spriteNum = Mathf.FloorToInt((woodSprites.Count - 1) * fullness);
		spriteRenderer.sprite = woodSprites[spriteNum];
	}
}
