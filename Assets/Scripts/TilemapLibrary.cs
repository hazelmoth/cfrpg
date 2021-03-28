using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;

// Stores a dictionary of scene object names and their respective tilemaps
public static class TilemapLibrary 
{
	private static IDictionary<string, Tilemap> groundMaps;
	private static IDictionary<string, Tilemap> groundCoverMaps;
	private static IDictionary<string, Tilemap> cliffMaps;

	private static string GroundTilemapTag = "GroundTilemap";
	private static string GroundCoverTilemapTag = "GroundCoverTilemap";
	private static string CliffsTilemapTag = "CliffsTilemap";

	// Finds all the currently loaded tilemaps and stores them with the name of their scene
	// (scenes need to be loaded to be added to the dictionary when this function is called)
	public static void BuildLibrary () 
	{
		groundMaps = new Dictionary<string, Tilemap> ();
		groundCoverMaps = new Dictionary<string, Tilemap>();
		cliffMaps = new Dictionary<string, Tilemap>();
		
		foreach (Tilemap tilemap in Object.FindObjectsOfType<Tilemap>()) 
		{
			if (!SceneObjectManager.SceneExists(SceneObjectManager.GetSceneIdForObject(tilemap.gameObject)))
			{
				Debug.LogWarning("There's a tilemap in the scene that isn't under a registered scene object!", tilemap.gameObject);
				continue;
			}

			if (tilemap.CompareTag(GroundTilemapTag))  
			{
				groundMaps.Add (SceneObjectManager.GetSceneIdForObject(tilemap.gameObject), tilemap);
			}
			else if (tilemap.CompareTag(GroundCoverTilemapTag))
			{
				groundCoverMaps.Add(SceneObjectManager.GetSceneIdForObject(tilemap.gameObject), tilemap);
			}
			else if (tilemap.CompareTag(CliffsTilemapTag))
			{
				cliffMaps.Add(SceneObjectManager.GetSceneIdForObject(tilemap.gameObject), tilemap);
			}
		}
	}

	public static Tilemap GetGroundTilemap (string scene) 
	{
		if (groundMaps.ContainsKey (scene))
			return groundMaps [scene];

		else if (SceneObjectManager.SceneExists(scene))
			Debug.LogWarning ("Couldn't find ground tilemap for requested scene (\"" + scene + "\") in TilemapLibrary.");

		else
			Debug.LogError("Given scene \"" + scene + "\" not found.");
		return null;
	}

	public static Tilemap GetGroundCoverTilemap(string scene)
	{
		if (groundCoverMaps.ContainsKey(scene))
			return groundCoverMaps[scene];

		else if (SceneObjectManager.SceneExists(scene))
			Debug.LogWarning("Couldn't find ground cover tilemap for requested scene (\"" + scene + "\") in TilemapLibrary.");

		else
			Debug.LogWarning("Given scene \"" + scene + "\" not found.");
		return null;
	}

	public static Tilemap GetCliffTilemap(string scene)
	{
		if (cliffMaps.ContainsKey(scene))
			return cliffMaps[scene];
		
		else if (SceneObjectManager.SceneExists(scene))
			Debug.LogWarning("Couldn't find cliff tilemap for requested scene (\"" + scene + "\") in TilemapLibrary.");

		else
			Debug.LogWarning("Given scene \"" + scene + "\" not found.");
		return null;
	}
}
