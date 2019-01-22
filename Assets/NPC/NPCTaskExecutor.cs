using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Contains functions that force an NPC to start or stop different tasks.
// These functions should be called by NPCScheduleFollower.
public class NPCTaskExecutor : MonoBehaviour {

	NPCNavigation nav;
	bool isWaitingForNavigationToFinish = false;

	// Use this for initialization
	void Start () {
		nav = this.GetComponent<NPCNavigation> ();
		if (nav == null)
			Debug.LogError ("This NPC seems to be missing an NPCNavigation component!", this);

		nav.NavigationCompleted += OnNavigationFinished;

	}
	
	// Update is called once per frame
	void Update () {
		
	}
		
	// Aimlessly move about
	public void Wander () {
		StopAllCoroutines ();
		StartCoroutine ("WanderCoroutine");
	}
	// Do nothing
	public void StandStill () {
		StopAllCoroutines ();
		// TODO a way to force the nav controller to stop
	}

	IEnumerator WanderCoroutine () {
		while (true) {
			// Walk to a random nearby tile
			nav.FollowPath (TileNavigationHelper.FindPath (transform.position, TileNavigationHelper.FindRandomNearbyPathTile (transform.position, 20)));
			isWaitingForNavigationToFinish = true;
			while (isWaitingForNavigationToFinish) {
				yield return null;
			}
			// Pause for a bit before walking again
			yield return new WaitForSeconds (Random.Range (1f, 5f));
		}
	}

	public void OnNavigationFinished () {
		isWaitingForNavigationToFinish = false;
	}
}
