using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableWorldMap
{
	public List<string> scenes;
	public List<SceneMap> sceneMaps;

	public SerializableWorldMap (WorldMap origin)
	{
		scenes = new List<string>();
		sceneMaps = new List<SceneMap>();

		foreach (string scene in origin.mapDict.Keys)
		{
			scenes.Add(scene);
			List<Vector2IntSerializable> locations = new List<Vector2IntSerializable>();
			List<SerializableMapUnit> mapUnits = new List<SerializableMapUnit>();

			foreach (Vector2Int location in origin.mapDict[scene].Keys)
			{
				SerializableMapUnit mapUnit = new SerializableMapUnit(origin.mapDict[scene][location]);
				locations.Add(location.ToSerializable());
				mapUnits.Add(mapUnit);
			}
			SceneMap sceneMap = new SceneMap(locations, mapUnits);
			sceneMaps.Add(sceneMap);
		}
	}


    [System.Serializable]
	public struct SceneMap
	{
		public List<Vector2IntSerializable> locations;
		public List<SerializableMapUnit> mapUnits;

		public SceneMap(List<Vector2IntSerializable> locations, List<SerializableMapUnit> mapUnits)
		{
			this.locations = locations;
			this.mapUnits = mapUnits;
		}
	}

	[System.Serializable]
	public struct SerializableMapUnit
	{
		public string entityId;
		public Vector2IntSerializable relativePosToEntityOrigin;
		public string groundMaterialId;

		public SerializableMapUnit (MapUnit origin)
		{
			entityId = origin.entityId;
			relativePosToEntityOrigin = origin.relativePosToEntityOrigin.ToSerializable();
			groundMaterialId = origin.groundMaterial.materialId;
		}
		
	}
}
public static class SerializableWorldMapExtension
{
    public static WorldMap ToNonSerializable(this SerializableWorldMap serializable)
    {
        WorldMap newMap = new WorldMap();
        newMap.mapDict = new Dictionary<string, Dictionary<Vector2Int, MapUnit>>();
        for (int i = 0; i < serializable.scenes.Count; i++)
        {
            string scene = serializable.scenes[i];
            SerializableWorldMap.SceneMap map = serializable.sceneMaps[i];
            Dictionary<Vector2Int, MapUnit> mapDict = new Dictionary<Vector2Int, MapUnit>();
            for (int j = 0; j < map.locations.Count; j++)
            {
                Vector2Int location = map.locations[j].ToVector2Int();
                MapUnit unit = map.mapUnits[j].ToNonSerializable();
                mapDict.Add(location, unit);
            }
            newMap.mapDict.Add(scene, mapDict);

        }
        return newMap;
    }
}
public static class SerializableMapUnitExtension
{
	public static MapUnit ToNonSerializable(this SerializableWorldMap.SerializableMapUnit source)
	{
		MapUnit mapUnit = new MapUnit();
		
		if (source.entityId == "")
		{
			mapUnit.entityId = null;
		}
		else
		{
			mapUnit.entityId = source.entityId;
		}

		mapUnit.groundMaterial = GroundMaterialLibrary.GetGroundMaterialById(source.groundMaterialId);
		mapUnit.relativePosToEntityOrigin = source.relativePosToEntityOrigin.ToVector2Int();
		return mapUnit;
	}
}
