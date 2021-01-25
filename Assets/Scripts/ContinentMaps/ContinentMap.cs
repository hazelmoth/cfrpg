using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ContinentMaps
{
    public class ContinentMap
    {
        public const int XSize = 25;
        public const int YSize = 25;

        public ContinentMap(ContinentBlueprint blueprint)
        {
            this.blueprint = blueprint;
            regions = new RegionMap[XSize, YSize];
        }

        public ContinentBlueprint blueprint;
        public RegionMap[,] regions;
    }
}
