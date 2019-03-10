using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

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
	}

	public static Vector2 GetCenterPositionOfTile (Vector2 tilePos) {
		return new Vector2 ((float)(tilePos.x + 0.5), (float)(tilePos.y + 0.5));
	}
	public static Vector2 WorldPosToScenePos (Vector2 worldPos, string sceneName) {
		Vector2 sceneRoot = SceneManager.GetSceneByName (sceneName).GetRootGameObjects () [0].transform.position;
		return worldPos - sceneRoot;
	}
	public static Vector2 ScenePosToWorldPos (Vector2 scenePos, string sceneName) {
		Vector2 sceneRoot = SceneManager.GetSceneByName (sceneName).GetRootGameObjects () [0].transform.position;
		return scenePos + sceneRoot;
	}
	public static TileBase GetTileAtPosition (float x, float y) {
		return instance.mainGroundTilemap.GetTile (new Vector3Int (Mathf.FloorToInt(x), Mathf.FloorToInt(y), 0));
	}
	public static TileBase GetTileAtPosition (float x, float y, string sceneName) {
		return TilemapLibrary.GetGroundTilemapForScene(sceneName).GetTile (new Vector3Int (Mathf.FloorToInt(x), Mathf.FloorToInt(y), 0));
	}
	public static void ChangeTile (int x, int y, TileBase tilePrefab) {
		instance.mainGroundTilemap.SetTile (new Vector3Int (x, y, 0), tilePrefab);
	}
	public static void ChangeTile (int x, int y, TileBase tilePrefab, string sceneName) {
		TilemapLibrary.GetGroundTilemapForScene(sceneName).SetTile (new Vector3Int (x, y, 0), tilePrefab);
	}
	public static void ClearWorldTilemap() {
		instance.mainGroundTilemap.ClearAllTiles ();
	}
	public static void ClearTilemap(string sceneName) {
		TilemapLibrary.GetGroundTilemapForScene (sceneName).ClearAllTiles ();
	}


	public static TileBase GetTileAtWorldPosition (float x, float y, string sceneName) {
		Tilemap map = TilemapLibrary.GetGroundTilemapForScene (sceneName);
		x -= map.transform.position.x;
		y -= map.transform.position.y;
		return map.GetTile (new Vector3Int (Mathf.FloorToInt(x), Mathf.FloorToInt(y), 0));
	}
	public static void RefreshAllPathTileSprites () {
		foreach (Tilemap map in TilemapLibrary.GetAllPathTilemaps()) {
			map.RefreshAllTiles ();
		}
	}
	public static BoundsInt GetBoundsOfScene (string sceneName) {
		Tilemap map = TilemapLibrary.GetGroundTilemapForScene (sceneName);
		map.CompressBounds ();
		return map.cellBounds;
	} 
}
