using UnityEngine;

public class TileMouseInputManager : MonoBehaviour {
	private bool isCheckingForInput;
	private bool inRange;
	private float maxDistanceFromPlayer;
	private static TileMouseInputManager instance;

	public static bool InRange => instance.inRange;


	// Use this for initialization
	private void Start () {
		instance = this;
		SetCheckingForInput (isCheckingForInput);
	}
	
	// Update is called once per frame
	private void LateUpdate () 
	{
		if (PauseManager.Paused)
		{
			return;
		}
		inRange = false;
		if (isCheckingForInput) 
		{
			Vector3Int CursorTilePos = GetTilePositionUnderCursor ();
			if (maxDistanceFromPlayer > 0 && Vector3.Distance (ActorRegistry.Get(PlayerController.PlayerActorId).actorObject.transform.position, GetTilePositionUnderCursor ()) > maxDistanceFromPlayer)
				TileMarkerController.HideTileMarkers ();
			else 
			{
				TileMarkerController.SetTileMarker (new Vector2Int(CursorTilePos.x, CursorTilePos.y));
				inRange = true;
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

	public static Vector3Int GetTilePositionUnderCursor () {
		Vector3 inputPos = Camera.main.ScreenToWorldPoint(new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 10f));
		int gridX = Mathf.FloorToInt (inputPos.x);
		int gridY = Mathf.FloorToInt (inputPos.y);
		return new Vector3Int (gridX, gridY, 0);
	}

	public static TileLocation GetTileUnderCursor (string scene)
	{
		Vector3Int pos = GetTilePositionUnderCursor();
		Vector2 localPos = TilemapInterface.WorldPosToScenePos(pos.ToVector2(), scene);
		Vector2Int finalPos = TilemapInterface.FloorToTilePos(localPos);

		return new TileLocation(finalPos, scene);
	}
}
