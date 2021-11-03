using System;
using System.Collections.Generic;
using System.Linq;
using ContentLibraries;
using UnityEngine;

[System.Serializable]
public class SerializableRegionMap
{
	public List<string> scenes;
	public List<SceneMap> sceneMaps;
	public List<SerializableScenePortal> portals;
	public Dictionary<string, RegionMap.ActorPosition> actors;
	public List<SavedDroppedItem> items;

	// Empty constructor needed for JSON deserialization
	public SerializableRegionMap() {}

	public SerializableRegionMap(RegionMap origin)
	{
		scenes = new List<string>();
		sceneMaps = new List<SceneMap>();
		portals = origin.scenePortals.ToList();
		actors = origin.actors;
		items = origin.droppedItems.ToList();

		foreach (string scene in origin.mapDict.Keys)
		{
			scenes.Add(scene);
			List<SerializableMapUnit> mapUnits = new List<SerializableMapUnit>();

			foreach (Vector2Int location in origin.mapDict[scene].Keys)
			{
				SerializableMapUnit mapUnit = new SerializableMapUnit(origin.mapDict[scene][location], location);
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
		// The position of this map unit in the scene
		public Vector2IntSerializable p;
		// The ID of the ground material
		public string g;
		// The ID of the ground cover material
		public string c;
		// The ID of the cliff material
		public string cl;
		// The tick of the last time this tile was moisturized
		public ulong mt;
		// The ID of the entity covering this tile, if there is one.
		public string e;
		// The relative position to the origin of the entity occupying this tile
		public Vector2IntSerializable rp;
		// The saved component data of the entity on this tile, if any
		public List<SavedComponentState> cd;


		public SerializableMapUnit (MapUnit origin, Vector2Int pos)
		{
			p = pos.ToSerializable();
			g = origin.groundMaterial?.materialId;
			c = origin.groundCover?.materialId;
			cl = origin.cliffMaterial?.materialId;
			mt = origin.lastMoisturizedTick;
			e = origin.entityId;
			rp = origin.relativePosToEntityOrigin.ToSerializable();
			cd = origin.savedComponents;

			Debug.Assert(origin.groundMaterial != null, "MapUnit shouldn't have a null ground material! " + pos);
		}
	}
}
public static class SerializableWorldMapExtension
{
	/**
	 * Returns a non-serializable version of this SerializableRegionMap.
	 * Throws an ArgumentNullException if parameter is null.
	 */
    public static RegionMap ToNonSerializable(this SerializableRegionMap serializable)
    {
	    if (serializable == null)
	    {
		    throw new ArgumentNullException(nameof(serializable), "Tried to unserialize a null RegionMap!");
	    }
        RegionMap newMap = new RegionMap();
        newMap.scenePortals = serializable.portals;
        newMap.actors = serializable.actors;
        newMap.mapDict = new Dictionary<string, Dictionary<Vector2Int, MapUnit>>();
        newMap.droppedItems = serializable.items;
        for (int i = 0; i < serializable.scenes.Count; i++)
        {
            string scene = serializable.scenes[i];
            SerializableRegionMap.SceneMap map = serializable.sceneMaps[i];
            Dictionary<Vector2Int, MapUnit> mapDict = new Dictionary<Vector2Int, MapUnit>();
            for (int j = 0; j < map.mapUnits.Count; j++)
            {
                MapUnit unit = map.mapUnits[j].ToNonSerializable();
                mapDict.Add(map.mapUnits[j].p.ToNonSerializable(), unit);
            }
            newMap.mapDict.Add(scene, mapDict);
        }
        return newMap;
    }
}
public static class SerializableMapUnitExtension
{
	public static MapUnit ToNonSerializable(this SerializableRegionMap.SerializableMapUnit source)
	{
		MapUnit mapUnit = new MapUnit();

		mapUnit.groundMaterial = ContentLibrary.Instance.GroundMaterials.Get(source.g);
		mapUnit.groundCover = string.IsNullOrEmpty(source.c) ? null : ContentLibrary.Instance.GroundMaterials.Get(source.c);
		mapUnit.cliffMaterial = string.IsNullOrEmpty(source.cl) ? null : ContentLibrary.Instance.GroundMaterials.Get(source.cl);
		mapUnit.lastMoisturizedTick = source.mt;
		mapUnit.entityId = source.e == "" ? null : source.e;
		mapUnit.relativePosToEntityOrigin = source.rp.ToNonSerializable();
		mapUnit.savedComponents = source.cd;
		return mapUnit;
	}
}
