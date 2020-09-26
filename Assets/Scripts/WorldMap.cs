using System.Collections.Generic;
using UnityEngine;

public class WorldMap
{
	// Maps scenes to dictionaries
	// Dictionaries map locations to map units
	public Dictionary<string, Dictionary<Vector2Int, MapUnit>> mapDict;

	public WorldMap()
	{
		mapDict = new Dictionary<string, Dictionary<Vector2Int, MapUnit>>();
	}
	public WorldMap(Dictionary<string, Dictionary<Vector2Int, MapUnit>> mapDict)
	{
		this.mapDict = mapDict;
	}
}
