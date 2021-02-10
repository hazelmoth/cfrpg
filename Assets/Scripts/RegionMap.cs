using System.Collections.Generic;
using UnityEngine;

public class RegionMap
{
	// Maps scenes to dictionaries
	public Dictionary<string, Dictionary<Vector2Int, MapUnit>> mapDict;

	public RegionMap()
	{
		mapDict = new Dictionary<string, Dictionary<Vector2Int, MapUnit>>();
	}
}
