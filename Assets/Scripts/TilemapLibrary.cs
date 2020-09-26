using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Stores a dictionary of scene object names and their respective tilemaps
public static class TilemapLibrary {
	private static IDictionary<string, Tilemap> groundMaps;

	private static string GroundTilemapTag = "GroundTilemap";

	// Finds all the currently loaded tilemaps and stores them with the name of their scene
	// (scenes need to be loaded to be added to the dictionary when this function is called)
	public static void BuildLibrary () {
		groundMaps = new Dictionary<string, Tilemap> ();
		foreach (Tilemap tilemap in Object.FindObjectsOfType<Tilemap>()) {
			if (tilemap.tag == GroundTilemapTag)  {
				if (!SceneObjectManager.SceneExists(SceneObjectManager.GetSceneIdForObject(tilemap.gameObject))) {
					Debug.LogWarning ("There's a tilemap in the scene that isn't under a registered scene object!");
					continue;
				}
				groundMaps.Add (SceneObjectManager.GetSceneIdForObject(tilemap.gameObject), tilemap);
			}
		}
	}

	public static Tilemap GetGroundTilemapForScene (string scene) {
		if (groundMaps.ContainsKey (scene))
			return groundMaps [scene];
		else if (SceneObjectManager.SceneExists(scene)){
			Debug.LogWarning ("Couldn't find ground tilemap for requested scene (\"" + scene + "\") in TilemapLibrary. Getting the tilemap directly.");
			if (SceneObjectManager.GetSceneObjectFromId(scene) != null)
				return SceneObjectManager.GetSceneObjectFromId (scene).GetComponentInChildren<Tilemap>();
		}
		Debug.LogWarning("No tilemap found in scene!");
		return null;
	}
}
