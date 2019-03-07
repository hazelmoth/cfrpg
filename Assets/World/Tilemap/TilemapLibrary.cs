using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

// Stores a dictionary of scene names and their respective tilemaps
public class TilemapLibrary : Object {

	static IDictionary<string, Tilemap> pathMaps;
	static IDictionary<string, Tilemap> groundMaps;

	static string PathTilemapTag = "PathTilemap";
	static string GroundTilemapTag = "GroundTilemap";

	// Finds all the currently loaded tilemaps and stores them with the name of their scene
	// (scenes need to be loaded to be added to the dictionary when this function is called)
public static void BuildLibrary () {
		pathMaps = new Dictionary<string, Tilemap> ();
		groundMaps = new Dictionary<string, Tilemap> ();
		foreach (Tilemap tilemap in FindObjectsOfType<Tilemap>()) {
			if (tilemap.tag == PathTilemapTag)  {
				pathMaps.Add (tilemap.gameObject.scene.name, tilemap);
			}
			else if (tilemap.tag == GroundTilemapTag)  {
				groundMaps.Add (tilemap.gameObject.scene.name, tilemap);
			}
			
		}
	}
	public static List<Tilemap> GetAllPathTilemaps () { 
		List<Tilemap> list = new List<Tilemap> ();
		list = pathMaps.Values.ToList ();
		return list;
	}
	public static Tilemap GetPathTilemapForScene (string scene) {
		if (pathMaps.ContainsKey (scene))
			return pathMaps [scene];
		else
			return null;
	}
	public static Tilemap GetGroundTilemapForScene (string scene) {
		if (groundMaps.ContainsKey (scene))
			return groundMaps [scene];
		else
			return null;
	}
}
