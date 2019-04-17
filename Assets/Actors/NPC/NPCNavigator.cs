using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Controls the NPC navigating from place to place.
// Should directly interface the NPCMovementController; nothing else should.
// This class should be controlled only by the functions in NPCTaskExecutor.
public class NPCNavigator : MonoBehaviour {

	public delegate void NPCNavigationEvent ();
	public event NPCNavigationEvent NavigationCompleted;

	NPCMovementController movement;
	public bool debugPath = false;

	// Use this for initialization
	void Awake () {
		movement = GetComponent<NPCMovementController> ();
		if (movement == null) {
			Debug.LogError ("NPC is missing a movement controller!");
		}
	}

	public void FollowPath (List<Vector2> worldPath) {
		StartCoroutine(FollowPathCoroutine (worldPath));
	}

	public void FollowPath (List<Vector2> path, string scene) {

		CancelNavigation ();
		// convert scene space back to world space
		List<Vector2> convertedPath = new List<Vector2>();
		foreach (Vector2 vector in path) {
			Vector2 newVector = TilemapInterface.ScenePosToWorldPos (vector, scene);
			convertedPath.Add (newVector);
		}
		StartCoroutine (FollowPathCoroutine (convertedPath));
	}
	public void CancelNavigation () {
		
		StopAllCoroutines ();
	}
	public void ForceDirection (Direction dir) {
		movement.SetDirection (dir);
	}

	void Walk (Vector2 destination, NPCNavigationEvent callback) {
		Vector2 startPos = transform.position;
		Vector2 endPos = destination;
		movement.SetDirection ((endPos - startPos).ToDirection());
		movement.SetWalking (true);
		StopCoroutine ("WalkCoroutine");
		StartCoroutine (WalkCoroutine (transform.position, Vector2.Distance(startPos, endPos), callback));
	}
	void Walk (Direction direction, float distance, NPCNavigationEvent callback) {
		Vector2 startPos = transform.position;
		Vector2 movementVector = direction.ToVector2() * distance;
		Vector2 endPos = startPos + movementVector;
		Walk (endPos, callback);
	}


	IEnumerator FollowPathCoroutine (List<Vector2> path) {
		if (debugPath)
			DebugPath(path);
		 
		foreach (Vector2 destination in path) {
			// Move the destination to the center of its tile
			Vector2 destCenter = TilemapInterface.GetCenterPositionOfTile (TilemapInterface.FloorToTilePos(destination));

			//Vector2 startPos = TilemapInterface.GetCenterPositionOfTile (Vector2Int.FloorToInt(transform.position));
			Vector2 startPos = transform.position;

			float distance = Vector2.Distance (startPos, destCenter);
			Walk (destCenter, null);
			while (Vector2.Distance(startPos, transform.position) <= distance) {
				yield return null;
			}
			movement.SetWalking (false);
		}
		if (NavigationCompleted != null)
			NavigationCompleted ();
	}
	IEnumerator WalkCoroutine (Vector2 startPos, float distance, NPCNavigationEvent callback) {
		while (Vector2.Distance(startPos, transform.position) <= distance) {
			yield return null;
		}
		if (callback != null)
			callback ();
		movement.SetWalking (false);
	}


	public void DebugPath (List<Vector2> worldPath) {
		

		LineRenderer liner = GetComponent<LineRenderer> ();
		if (liner == null)
			liner = gameObject.AddComponent<LineRenderer> ();
		
		Vector3[] linePoints = new Vector3[worldPath.Count];

		for(int i = 0; i < worldPath.Count; i++) {
			linePoints [i] = new Vector3 (worldPath [i].x + 0.5f, worldPath [i].y + 0.5f, -2f);
		}
		liner.startWidth = 0.1f;
		liner.endWidth = 0.1f;
		liner.positionCount = linePoints.Length;
		liner.startColor = new Color (Random.value, Random.value, Random.value, 0.6f);
		liner.endColor = new Color (Random.value, Random.value, Random.value, 0.6f);
		liner.material = (Material)Resources.Load ("DebugMaterial");
		liner.SetPositions (linePoints);
		NavigationCompleted += HideDebugPath;
	}
	public void HideDebugPath () {
		LineRenderer liner = gameObject.GetComponent<LineRenderer> ();
		if (liner != null) {
			GameObject.Destroy (liner);
		}
	}


}
