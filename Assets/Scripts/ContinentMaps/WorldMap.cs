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
        public readonly IList<RegionInfo> regions;
        private readonly ImmutableDictionary<string, RegionInfo> idToRegion;

        /// Creates a continent map with the given name and dimensions, without any regions initially generated.
        public WorldMap(string name, Vector2Int dimensions, IList<RegionInfo> regions)
        {
            Debug.Assert(!regions.Where(region => region == null).Any());
            if(regions.Where(region => region.Id == null).Any())
            {
                Debug.LogError("Region with null ID!\n" + regions.Where(region => region.Id == null).First().ToString());
            }


            continentName = name;
            this.dimensions = dimensions;
            this.regions = regions.ToList();
            idToRegion = regions.ToImmutableDictionary(region => region.Id);
        }

        public bool Contains(string regionId) => idToRegion.ContainsKey(regionId);
        public RegionInfo Get(string id) => idToRegion[id];
    }
}
