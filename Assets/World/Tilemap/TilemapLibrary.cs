using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

// Stores a dictionary of scene names and their respective tilemaps
public class TilemapLibrary : Object {

	static IDictionary<string, Tilemap> groundMaps;

	static string GroundTilemapTag = "GroundTilemap";

	// Finds all the currently loaded tilemaps and stores them with the name of their scene
	// (scenes need to be loaded to be added to the dictionary when this function is called)
	public static void BuildLibrary () {
		groundMaps = new Dictionary<string, Tilemap> ();
		foreach (Tilemap tilemap in FindObjectsOfType<Tilemap>()) {
			if (tilemap.tag == GroundTilemapTag)  {
				groundMaps.Add (tilemap.gameObject.scene.name, tilemap);
			}
			
		}
	}

	public static Tilemap GetGroundTilemapForScene (string scene) {
		if (groundMaps.ContainsKey (scene))
			return groundMaps [scene];
		else if (SceneManager.GetSceneByName(scene).IsValid()){
			Debug.LogWarning ("Couldn't find ground tilemap for requested scene in TilemapLibrary. Getting the tilemap directly.");
			return SceneManager.GetSceneByName (scene).GetRootGameObjects() [0].GetComponentInChildren<Tilemap>();
		}
		return null;
	}
}
