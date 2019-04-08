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

	// Use this for initialization
	void Awake () {
		movement = GetComponent<NPCMovementController> ();
		if (movement == null) {
			Debug.LogError ("NPC is missing a movement controller!");
		}
	}

	public void FollowPath (List<Vector2> path) {
		string scene = GetComponent<NPC> ().ActorCurrentScene;
		FollowPath (path, scene);
	}
	public void FollowPath (List<Vector2> path, string scene) {
		// convert scene space back to world space
		List<Vector2> convertedPath = new List<Vector2>();
		foreach (Vector2 vector in path) {
			Vector2 newVector = TilemapInterface.ScenePosToWorldPos (vector, scene);
			convertedPath.Add (newVector);
		}
		StartCoroutine (FollowPathCoroutine (convertedPath));
	}
	public void ForceDirection (Direction dir) {
		movement.SetDirection (dir);
	}
	void Walk (Vector2 destination) {
		Vector2 startPos = transform.position;
		Vector2 endPos = destination;
		movement.SetDirection ((endPos - startPos).ToDirection());
		movement.SetWalking (true);
		StopCoroutine ("FinishWalk");
		StartCoroutine (WalkCoroutine (transform.position, Vector2.Distance(startPos, endPos)));
	}
	void Walk (Direction direction, float distance) {
		Vector2 startPos = transform.position;
		Vector2 movementVector = direction.ToVector2() * distance;
		Vector2 endPos = startPos + movementVector;
		Walk (endPos);
	}
	IEnumerator FollowPathCoroutine (List<Vector2> path) {
		foreach (Vector2 destination in path) {
			// Move the destination to the center of its tile
			Vector2 destCenter = TilemapInterface.GetCenterPositionOfTile (destination);

			Vector2 startPos = transform.position;
			float distance = Vector2.Distance (startPos, destCenter);
			movement.SetDirection ((destCenter - startPos).ToDirection ());
			movement.SetWalking (true);
			while (Vector2.Distance(startPos, transform.position) <= distance) {
				yield return null;
			}
			movement.SetWalking (false);
		}
		if (NavigationCompleted != null)
			NavigationCompleted ();
	}
	IEnumerator WalkCoroutine (Vector2 startPos, float distance) {
		while (Vector2.Distance(startPos, transform.position) <= distance) {
			yield return null;
		}
		movement.SetWalking (false);
	}


}
