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
			Vector3Int CursorTilePos = GetTilePositionUnderCursor ();
			if (maxDistanceFromPlayer > 0 && Vector3.Distance (Player.instance.transform.position, GetTilePositionUnderCursor ()) > maxDistanceFromPlayer)
				TileMarkerController.HideTileMarkers ();
			else {
				TileMarkerController.SetTileMarker (new Vector2Int(CursorTilePos.x, CursorTilePos.y));
				if (Input.GetMouseButtonDown (0)) {
					if (OnTileClicked != null)
						OnTileClicked (GetTilePositionUnderCursor());
				}
			}
		}
	}

	public static void SetCheckingForInput (bool checkForInput) {
		instance.isCheckingForInput = checkForInput;
		if (!checkForInput)
			TileMarkerController.HideTileMarkers ();
	}

	public static void SetMaxDistance (float dist) {
		instance.maxDistanceFromPlayer = dist;
	}

	static Vector3Int GetTilePositionUnderCursor () {
		Vector3 inputPos = Camera.main.ScreenToWorldPoint(new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 10f));
		int gridX = Mathf.FloorToInt (inputPos.x);
		int gridY = Mathf.FloorToInt (inputPos.y);
		return new Vector3Int (gridX, gridY, 0);
	}
}
