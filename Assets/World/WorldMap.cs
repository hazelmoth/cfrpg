using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMap
{
	// Maps scenes to dictionaries
	// Dictionaries map locations to map units
	public Dictionary<string, Dictionary<Vector2Int, MapUnit>> mapDict;
}
