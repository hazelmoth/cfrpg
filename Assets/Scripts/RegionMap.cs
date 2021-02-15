using System.Collections.Generic;
using UnityEngine;

public class RegionMap
{
	// Maps scenes to dictionaries
	public IDictionary<string, Dictionary<Vector2Int, MapUnit>> mapDict;
	public IList<SerializableScenePortal> scenePortals;

	public RegionMap()
	{
		mapDict = new Dictionary<string, Dictionary<Vector2Int, MapUnit>>();
		scenePortals = new List<SerializableScenePortal>();
	}
}
