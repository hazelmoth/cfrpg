using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMouseInputManager : MonoBehaviour {

	bool isCheckingForInput;
	float maxDistanceFromPlayer;
	public delegate void TileClickEvent (Vector3Int location);
	public static event TileClickEvent OnTileClicked;
	static TileMouseInputManager instance;

	// Use this for initialization
	void Start () {
		instance = this;
		SetCheckingForInput (isCheckingForInput);
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if (isCheckingForInput) {
			if (maxDistanceFromPlayer > 0 && Vector3.Distance (Player.instance.transform.position, SelectedTileMarker.CurrentPosition) > maxDistanceFromPlayer)
				SelectedTileMarker.SetVisible (false);
			else {
				SelectedTileMarker.SetVisible (true);
				if (Input.GetMouseButtonDown (0)) {
					if (OnTileClicked != null)
						OnTileClicked (SelectedTileMarker.CurrentPosition);
				}
			}
		}
	}

	public static void SetCheckingForInput (bool checkForInput) {
		instance.isCheckingForInput = checkForInput;
		SelectedTileMarker.SetFollowMouse(checkForInput);
		SelectedTileMarker.SetVisible (checkForInput);
	}

	public static void SetMaxDistance (float dist) {
		instance.maxDistanceFromPlayer = dist;
	}
}
