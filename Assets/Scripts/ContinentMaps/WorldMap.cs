using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ContinentMaps
{
    public class WorldMap
    {
        public Vector2Int dimensions;
        public string continentName;
        public RegionMap[,] regions;
        public RegionInfo[,] regionInfo;

        // Creates a continent map with the given name and dimensions, without any regions initially generated.
        public WorldMap(string name, Vector2Int dimensions, RegionInfo[,] regionInfo)
        {
            continentName = name;
            this.dimensions = dimensions;
            regions = new RegionMap[dimensions.x, dimensions.y];
            this.regionInfo = regionInfo;
        }
    }
}
