
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ContinentMaps
{
    [System.Serializable]
    public class SerializableWorldMap
    {
        public Vector2IntSerializable dimensions;
        public string continentName;
        public List<SerializableRegion> regions;

        public WorldMap ToNonSerializable()
        {
            List<Region> nonSerializableRegions = regions.Select(
                region => new Region
                {
                    info = region.info,
                    data = region.data.ToNonSerializable()
                }).ToList();

            return new WorldMap(continentName, dimensions.ToNonSerializable(), nonSerializableRegions);
        }
    }

    public static class ContinentMapExtension
    {
        // Creates a serializable continent map from the given continent map.
        public static SerializableWorldMap ToSerializable(this WorldMap original)
        {
            SerializableWorldMap serializable = new SerializableWorldMap
            {
                continentName = original.continentName,
                dimensions = original.dimensions.ToSerializable(),
                regions = original.regions.Select(region => new SerializableRegion(region)).ToList()
            };

            Debug.Assert(serializable.regions.Count == original.regions.Count);
            return serializable;
        }
    }
}
