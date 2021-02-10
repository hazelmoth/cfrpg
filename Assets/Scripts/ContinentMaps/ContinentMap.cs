using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ContinentMaps
{
    public class ContinentMap
    {
        public Vector2Int dimensions;
        public string continentName;
        public RegionMap[,] regions;
        public RegionType[,] regionInfo;

        // Creates a continent map with the given name and dimensions, without any regions initially generated.
        public ContinentMap(string name, Vector2Int dimensions, RegionType[,] regionInfo)
        {
            continentName = name;
            this.dimensions = dimensions;
            regions = new RegionMap[dimensions.x, dimensions.y];
            this.regionInfo = regionInfo;
        }
    }
}
