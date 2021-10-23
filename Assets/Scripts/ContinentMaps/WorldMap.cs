using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using UnityEngine;

namespace ContinentMaps
{
    public class WorldMap
    {
        public readonly Vector2Int dimensions;
        public readonly string continentName;
        public readonly IList<Region> regions;
        private readonly ImmutableDictionary<string, Region> idToRegion;

        /// Creates a continent map with the given name and regions.
        public WorldMap(string name, Vector2Int dimensions, IList<Region> regions)
        {
            Debug.Assert(regions.All(region => region != null));
            if (regions.Any(region => region.info.Id == null))
            {
                Debug.LogError("Region with null ID!\n" + regions.First(region => region.info.Id == null).info);
            }


            continentName = name;
            this.dimensions = dimensions;
            this.regions = regions.ToList();
            idToRegion = regions.ToImmutableDictionary(region => region.info.Id);
        }

        public bool Contains(string regionId) => idToRegion.ContainsKey(regionId);
        public Region Get(string id) => idToRegion[id];
    }
}
