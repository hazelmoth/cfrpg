using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Stores the locations of entities for loaded scenes
public class EntityMap : MonoBehaviour
{
	// Maps scenes to map arrays
	// Arrays store entity IDs
	static Dictionary<string, string[,]> mapDict;

	public static void InitializeMap () {
		foreach (string scene in SceneFinder.GetLoadedSceneNames()) {
			BoundsInt bounds = TilemapInterface.GetBoundsOfScene (scene);
			int yRange = bounds.yMax - bounds.yMin;
			int xRange = bounds.xMax - bounds.xMin;
			string[,] array = new string[yRange, xRange];
			mapDict.Add (scene, array);
		}
	}
}
