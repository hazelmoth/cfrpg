using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using UnityEngine;

namespace ContinentMaps
{
    public class WorldMap
    {
        public readonly Vector2Int dimensions;
        public readonly string continentName;
        public readonly IList<RegionInfo> regions;
        private readonly ImmutableDictionary<string, RegionInfo> idToRegion;

        /// Creates a continent map with the given name and dimensions, without any regions initially generated.
        public WorldMap(string name, Vector2Int dimensions, IList<RegionInfo> regions)
        {
            continentName = name;
            this.dimensions = dimensions;
            this.regions = regions;
            idToRegion = regions.ToImmutableDictionary(region => region.Id);
        }

        public bool Contains(string regionId) => idToRegion.ContainsKey(regionId);
        public RegionInfo Get(string id) => idToRegion[id];
    }
}
