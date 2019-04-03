using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour, InteractableObject
{
	[SerializeField] int logYield = 3;
	// how much length the logs that fall out of the tree are spread around
	[SerializeField] float trunkHeight = 4f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	List<Vector2> GetRelativeWoodSpawnPositions () {
		List<Vector2> list = new List<Vector2> ();
		for (int i = 0; i < logYield; i++) {
			float y = trunkHeight / (float)logYield * i;
			list.Add (new Vector2 (0f, y));
		}
		return list;
	}
	// Makes the tree fall apart into wood
	void Break () {
		foreach (Vector2 pos in GetRelativeWoodSpawnPositions()) {
			DroppedItem item = DroppedItemSpawner.SpawnItem ("log", pos + (Vector2)transform.localPosition, "World");
			item.InitiateFakeFall (pos.y);
		}
		GameObject.Destroy (this.gameObject);
	}

	public void OnInteract () {
		Break ();
	}
}
