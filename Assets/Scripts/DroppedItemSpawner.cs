using UnityEngine;

public class DroppedItemSpawner : MonoBehaviour
{
	[SerializeField] private GameObject droppedItemPrefab;
	private static DroppedItemSpawner instance;

    // Start is called before the first frame update
    private void Start()
    {
		instance = this;
    }

	public static DroppedItem SpawnItem (Item item, Vector2 position, string scene, bool randomlyShiftPosition) {
		if (randomlyShiftPosition) {
			position = new Vector2 (position.x + Random.Range (-0.5f, 0.5f), position.y + Random.Range (-0.5f, 0.5f));
		}
		return SpawnItem (item, position, scene);
	}

	public static DroppedItem SpawnItem (Item item, Vector2 position, string scene) {
		// make sure we've been initialized
		if (instance == null) {
			instance = GameObject.FindObjectOfType<DroppedItemSpawner> ();

			if (instance == null)
				return null;
		}

		// Check if this is the ID of an itemized actor, and if so spawn that actor instead
		if (item.id != null && item.id.Contains(":") && item.id.Split(':').Length == 2 && item.id.Split(':')[0] == "actor")
		{
			string actorId = item.id.Split(':')[1];
			ActorSpawner.Spawn(actorId, position, scene);
			return null;
		}
		
		GameObject newItem = GameObject.Instantiate (instance.droppedItemPrefab);
		newItem.GetComponent<DroppedItem> ().SetItem (item);

		newItem.transform.SetParent (SceneObjectManager.GetSceneObjectFromId(scene).transform);
		newItem.transform.localPosition = position;
		return newItem.GetComponent<DroppedItem> ();
	}
}
