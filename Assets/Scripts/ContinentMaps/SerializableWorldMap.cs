
using System.Linq;
using UnityEngine;

namespace ContinentMaps
{
    [System.Serializable]
    public class SerializableWorldMap
    {
        public Vector2IntSerializable dimensions;
        public string continentName;
        public RegionInfo[] regionInfo;
        public SerializableRegionMap[] regions;

        public WorldMap ToNonSerializable()
        {
            WorldMap newMap = new WorldMap(continentName, dimensions.ToNonSerializable(), regionInfo);
            
            for (int i = 0; i < regions.Length; i++)
            {
                if (regions[i] != null)
                    newMap.regions[i].regionData = regions[i].ToNonSerializable();
            }
            return newMap;
        }
    }

    public static class ContinentMapExtension
    {
        // Creates a serializable continent map from the given continent map.
        public static SerializableWorldMap ToSerializable(this WorldMap original)
        {
            SerializableWorldMap serializable = new SerializableWorldMap();
            serializable.continentName = original.continentName;
            serializable.dimensions = original.dimensions.ToSerializable();
            serializable.regionInfo = original.regions.ToArray();
            serializable.regions = serializable.regionInfo
                .Select(info => info != null ? new SerializableRegionMap(info.regionData) : null)
                .ToArray();
            Debug.Assert(serializable.regions.Length == serializable.regionInfo.Length);
            return serializable;
        }
    }
}
