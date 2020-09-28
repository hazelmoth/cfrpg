using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableWorldMap
{
	public List<string> scenes;
	public List<SceneMap> sceneMaps;

	public SerializableWorldMap() {}

	public SerializableWorldMap(WorldMap origin)
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
				SerializableMapUnit mapUnit = new SerializableMapUnit(origin.mapDict[scene][location], location);
				locations.Add(location.ToSerializable());
				mapUnits.Add(mapUnit);
			}
			SceneMap sceneMap = new SceneMap(mapUnits);
			sceneMaps.Add(sceneMap);
		}
	}


    [System.Serializable]
	public struct SceneMap
	{
		public List<SerializableMapUnit> mapUnits;

		public SceneMap(List<SerializableMapUnit> mapUnits)
		{
			this.mapUnits = mapUnits;
		}
	}

	[System.Serializable]
	public struct SerializableMapUnit
	{
		// The ID of the ground material
		public string g;
		// The ID of the ground cover material
		public string c;
		// The position of this map unit in the scene
		public Vector2IntSerializable p;
		// The ID of the entity covering this tile, if there is one.
		public string e;
		// The relative position to the origin of the entity occupying this tile
		public Vector2IntSerializable rp;


		public SerializableMapUnit (MapUnit origin, Vector2Int pos)
		{
			e = origin.entityId;
			p = pos.ToSerializable();
			rp = origin.relativePosToEntityOrigin.ToSerializable();
			g = origin.groundMaterial.materialId;
			c = origin.groundCover == null ? null : origin.groundCover.materialId;
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
            for (int j = 0; j < map.mapUnits.Count; j++)
            {
                MapUnit unit = map.mapUnits[j].ToNonSerializable();
                mapDict.Add(map.mapUnits[j].p.ToVector2Int(), unit);
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
		
		mapUnit.entityId = source.e == "" ? null : source.e;

		mapUnit.groundMaterial = ContentLibrary.Instance.GroundMaterials.Get(source.g);
		mapUnit.groundCover = source.c == "" ? null : ContentLibrary.Instance.GroundMaterials.Get(source.c);

		mapUnit.relativePosToEntityOrigin = source.rp.ToVector2Int();
		return mapUnit;
	}
}
