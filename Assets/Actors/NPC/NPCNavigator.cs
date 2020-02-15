using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// An NPCNavigator controls its actor as it moves along a given path.
// It does not perform any pathfinding on its own.
// Directly interfaces the NPCMovementController; nothing else should.
public class NPCNavigator : MonoBehaviour {

	public delegate void NPCNavigationEvent ();
	public event NPCNavigationEvent NavigationCompleted;

	private NPCMovementController movement;
	private Actor actor;
	public bool debugPath = false;

	// Use this for initialization
	void Awake () {
		movement = GetComponent<NPCMovementController> ();
		if (movement == null) {
			Debug.LogError ("NPC is missing a movement controller!");
		}
		actor = GetComponent<Actor>();
	}

	public void FollowPath (List<Vector2> path, string scene)
	{
		FollowPath(path, scene, null);
	}
	public void FollowPath (List<Vector2> path, string scene, NPCNavigationEvent callback) {

		CancelNavigation ();
		// convert scene space back to world space
		List<Vector2> convertedPath = new List<Vector2>();
		foreach (Vector2 vector in path) {
			Vector2 newVector = TilemapInterface.ScenePosToWorldPos (vector, scene);
			convertedPath.Add (newVector);
		}
		StartCoroutine (FollowPathCoroutine (convertedPath, callback));
	}
	public void CancelNavigation ()
	{
		StopAllCoroutines();
		movement.SetWalking(false);
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


	IEnumerator FollowPathCoroutine (List<Vector2> worldPath, NPCNavigationEvent callback) {
		if (debugPath)
			DebugPath(worldPath);
		 
		foreach (Vector2 destination in worldPath) {
			// Move the destination to the center of its tile
			Vector2 destCenter = TilemapInterface.GetCenterPositionOfTile (TilemapInterface.FloorToTilePos(destination));

			//Vector2 startPos = TilemapInterface.GetCenterPositionOfTile (Vector2Int.FloorToInt(transform.position));
			Vector2 startPos = transform.position;

			float distance = Vector2.Distance (startPos, destCenter);
			bool walkFinished = false;

			Walk (destCenter, () => walkFinished = true);
			while (!walkFinished) {
				yield return null;
			}
			movement.SetWalking (false);
		}
		NavigationCompleted?.Invoke();
		callback?.Invoke();
	}
	IEnumerator WalkCoroutine (Vector2 startPos, float distance, NPCNavigationEvent callback) {
		while (Vector2.Distance(startPos, transform.position) <= distance) {
			// TODO make sure we're always pointing the right way
			if (ObstacleDetectionSystem.CheckForObstacles(actor, actor.Direction)) {
				movement.SetWalking(false);
			} else {
				movement.SetWalking(true);
			}
			yield return null;
		}
		callback?.Invoke();
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
