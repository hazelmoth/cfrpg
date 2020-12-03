using System;
using UnityEngine;

namespace ContinentMaps
{
    public class ContinentManager : MonoBehaviour
    {
        private const int regionSize = 100;
        private ContinentMap map;

        public void Load(ContinentMap map)
        {
            this.map = map;
        }

        public void GetRegion(int x, int y, Action<RegionMap> callback)
        {
            if (map.regions[x, y] != null)
            {
                callback(map.regions[x, y]);
            }
            else
            {
                // This region hasn't been generated yet. We'll do the honors.
                WorldMapGenerator.StartGeneration(regionSize, regionSize, Time.time, HandleGenerationComplete, this);

                void HandleGenerationComplete(bool success, RegionMap map)
                {
                    if (!success) Debug.LogError("Region generation failed!");
                    this.map.regions[x, y] = map;
                    callback?.Invoke(map);
                }
            }
        }
    }
}