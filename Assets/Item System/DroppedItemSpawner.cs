using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DroppedItemSpawner : MonoBehaviour
{
	[SerializeField] GameObject droppedItemPrefab;
	static DroppedItemSpawner instance;

    // Start is called before the first frame update
    void Start()
    {
		instance = this;
    }

	public static DroppedItem SpawnItem (string itemId, Vector2 position, string scene, bool randomlyShiftPosition) {
		if (randomlyShiftPosition) {
			position = new Vector2 (position.x + Random.Range (-0.5f, 0.5f), position.y + Random.Range (-0.5f, 0.5f));
		}
		return SpawnItem (itemId, position, scene);
	}
	public static DroppedItem SpawnItem (string itemId, Vector2 position, string scene) {
		// make sure we've been initialized
		if (instance == null) {
			instance = GameObject.FindObjectOfType<DroppedItemSpawner> ();

			if (instance == null)
				return null;
		}

		GameObject newItem = GameObject.Instantiate (instance.droppedItemPrefab);
		newItem.GetComponent<DroppedItem> ().SetItem (itemId);

		newItem.transform.SetParent (SceneManager.GetSceneByName (scene).GetRootGameObjects() [0].transform);
		newItem.transform.localPosition = position;
		return newItem.GetComponent<DroppedItem> ();
	}
}
