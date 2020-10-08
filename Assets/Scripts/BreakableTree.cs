using System.Collections.Generic;
using UnityEngine;

public class BreakableTree : BreakableObject
{
	[SerializeField] private int logYield = 3;
	// how much length the logs that fall out of the tree are spread around
	[SerializeField] private float trunkHeight = 4f;

	private List<Vector2> GetRelativeWoodSpawnPositions () {
		List<Vector2> list = new List<Vector2> ();
		for (int i = 0; i < logYield; i++) {
			float y = trunkHeight / (float)logYield * i;
			list.Add (new Vector2 (0f, y));
		}
		return list;
	}
	// Makes the tree fall apart into wood
	protected override void DropItems ()
	{
		List<DroppedItem> droppedItems = new List<DroppedItem>();

		foreach (Vector2 pos in GetRelativeWoodSpawnPositions()) {
			DroppedItem item = DroppedItemSpawner.SpawnItem (new ItemStack("wood", 1), pos + (Vector2)transform.localPosition, SceneObjectManager.WorldSceneId);
			item.InitiateFakeFall (pos.y);
			droppedItems.Add(item);
		}
		RaiseDropItemsEvent(droppedItems);
	}
}
