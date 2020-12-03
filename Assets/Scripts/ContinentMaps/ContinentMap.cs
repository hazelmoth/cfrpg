using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ContinentMaps
{
    public class ContinentMap
    {
        const int xSize = 25;
        const int ySize = 25;

        public ContinentMap(ContinentBlueprint blueprint)
        {
            this.blueprint = blueprint;
            regions = new RegionMap[xSize, ySize];
        }

        public ContinentBlueprint blueprint;
        public RegionMap[,] regions;
    }
}
