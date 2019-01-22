using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapInterface : MonoBehaviour {

	Tilemap mainGroundTilemap = null;
	Tilemap mainPathTilemap = null;
	Tilemap currentGroundTilemap = null;
	Tilemap currentPathTilemap = null;
	static TilemapInterface instance;


	// Use this for initialization
	void Start () {
		SceneLoader.OnScenesLoaded += LoadTilemaps; // Rebuild the library after loading scenes
		LoadTilemaps();
		instance = this;
	}
	void LoadTilemaps () {
		TilemapLibrary.BuildLibrary ();
		mainGroundTilemap = TilemapLibrary.GetGroundTilemapForScene("World");
		mainPathTilemap = TilemapLibrary.GetPathTilemapForScene("World");
	}

	public static Vector2 GetCenterPositionOfTile (Vector2 tilePos) {
		return new Vector2 ((float)(tilePos.x + 0.5), (float)(tilePos.y + 0.5));
	}
	public static TileBase GetTileAtWorldPosition (float x, float y) {
		return instance.mainGroundTilemap.GetTile (new Vector3Int (Mathf.FloorToInt(x), Mathf.FloorToInt(y), 0));
	}
	public static TileBase GetTileAtWorldPosition (float x, float y, string sceneName) {
		return TilemapLibrary.GetGroundTilemapForScene(sceneName).GetTile (new Vector3Int (Mathf.FloorToInt(x), Mathf.FloorToInt(y), 0));
	}
	public static void ChangeTile (int x, int y, TileBase tilePrefab) {
		instance.mainGroundTilemap.SetTile (new Vector3Int (x, y, 0), tilePrefab);
	}
	public static void ChangeTile (int x, int y, TileBase tilePrefab, string sceneName) {
		TilemapLibrary.GetGroundTilemapForScene(sceneName).SetTile (new Vector3Int (x, y, 0), tilePrefab);
	}

	public static Tilemap GetPathTilemap () {
		return instance.mainPathTilemap;
	}
	public static Tilemap GetPathTilemap (string sceneName) {
		return TilemapLibrary.GetPathTilemapForScene (sceneName);
	}
	public static TileBase GetPathTileAtWorldPosition (float x, float y) {
		x -= instance.mainPathTilemap.transform.position.x;
		y -= instance.mainPathTilemap.transform.position.y;
		return instance.mainPathTilemap.GetTile (new Vector3Int (Mathf.FloorToInt(x), Mathf.FloorToInt(y), 0));
	}
	public static TileBase GetPathTileAtWorldPosition (float x, float y, string sceneName) {
		Tilemap map = TilemapLibrary.GetPathTilemapForScene (sceneName);
		x -= map.transform.position.x;
		y -= map.transform.position.y;
		return map.GetTile (new Vector3Int (Mathf.FloorToInt(x), Mathf.FloorToInt(y), 0));
	}
	public static void RefreshAllPathTileSprites () {
		foreach (Tilemap map in TilemapLibrary.GetAllPathTilemaps()) {
			map.RefreshAllTiles ();
		}
	}
}
