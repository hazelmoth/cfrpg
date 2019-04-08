using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A plant that can be harvested through an interaction rather than just destroying it
public class HarvestablePlant : MonoBehaviour, InteractableObject
{
	//TODO consolidate harvestable plants with breakable plants

	//TODO support for swapping sprites after harvesting

	[SerializeField] string droppedItemId; // TODO support for dropping multiple, different items
	[SerializeField] int maxDropNumber;
	[SerializeField][Range(0, 1)] float dropProbability;
	// how high the items drop from when harvested
	[SerializeField] float dropHeight = 0.75f;
	[SerializeField] bool destroyOnHarvest = false;

	void Harvest () {
		if (ItemManager.GetItemById(droppedItemId) == null) {
			Debug.LogWarning ("The drop item ID for this harvestable plant is invalid!");
			return;
		}
		for (int i = 0; i < maxDropNumber; i++) {
			Vector2 dropPosition = new Vector2 (transform.localPosition.x, transform.localPosition.y + dropHeight);
			DroppedItem item = DroppedItemSpawner.SpawnItem (droppedItemId, dropPosition, SceneObjectManager.WorldSceneId);
			item.InitiateFakeFall (dropHeight);
		}
		if (destroyOnHarvest) {
			Vector2Int tilePos = new Vector2Int ((int)transform.position.x, (int)transform.position.y);
			Vector2 localPos = TilemapInterface.WorldPosToScenePos (tilePos, SceneObjectManager.WorldSceneId);
			WorldMapManager.RemoveEntityAtPoint (new Vector2Int ((int)localPos.x, (int)localPos.y), SceneObjectManager.WorldSceneId);
		}
	}

	public void OnInteract () {
		Harvest ();

	}
}
