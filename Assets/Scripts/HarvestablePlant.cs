using UnityEngine;

// A plant that can be harvested through an interaction rather than just destroying it
public class HarvestablePlant : MonoBehaviour, IInteractableObject
{
	//TODO consolidate harvestable plants with breakable plants

	//TODO support for swapping sprites after harvesting

	[SerializeField] private string droppedItemId; // TODO support for dropping multiple, different items
	[SerializeField] private int maxDropNumber;
	[SerializeField][Range(0, 1)] private float dropProbability;
	// how high the items drop from when harvested
	[SerializeField] private float dropHeight = 0.75f;
	[SerializeField] private bool destroyOnHarvest = false;

	public void Harvest () {
		DroppedItem item;
		Harvest (out item);
	}
	public void Harvest (out DroppedItem droppedItem) 
	{
		droppedItem = null;

		if (ContentLibrary.Instance.Items.Get(droppedItemId) == null) 
		{
			Debug.LogWarning ("The drop item ID for this harvestable plant is invalid!");
			return;
		}
		for (int i = 0; i < maxDropNumber; i++) 
		{
			Vector2 dropPosition = new Vector2 (transform.localPosition.x, transform.localPosition.y + dropHeight);
			DroppedItem item = DroppedItemSpawner.SpawnItem (new Item(droppedItemId, 1), dropPosition, SceneObjectManager.WorldSceneId);
			droppedItem = item;
			item.InitiateFakeFall (dropHeight);
		}

		if (destroyOnHarvest) 
		{
			Vector2Int tilePos = new Vector2Int ((int)transform.position.x, (int)transform.position.y);
			Vector2 localPos = TilemapInterface.WorldPosToScenePos (tilePos, SceneObjectManager.WorldSceneId);
			WorldMapManager.RemoveEntityAtPoint (new Vector2Int ((int)localPos.x, (int)localPos.y), SceneObjectManager.WorldSceneId);
		} else {
			// Sprite swapping
		}
	}

	public void OnInteract () {
		Harvest ();
	}
}
