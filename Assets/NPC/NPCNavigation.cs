using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Controls the NPC navigating from place to place.
// Should directly interface the NPCMovementController; nothing else should.
// This class should be controlled by the functions in NPCTaskExecutor.
public class NPCNavigation : MonoBehaviour {

	public delegate void NPCNavigationEvent ();
	public event NPCNavigationEvent NavigationCompleted;

	NPCMovementController movement;

	// Use this for initialization
	void Start () {
		movement = GetComponent<NPCMovementController> ();
		if (movement == null) {
			Debug.LogError ("NPC is missing a movement controller!");
		}
	}

	public void FollowPath (List<Vector2> path) {
		StartCoroutine (FollowPathCoroutine (path));
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
