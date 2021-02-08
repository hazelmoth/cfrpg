using System;
using UnityEngine;

namespace ContinentMaps
{
    // Stores the currently-loaded Continent Map, allowing the client
    // to get particular region maps, generating them as needed.
    public static class ContinentManager
    {
        private const int RegionSize = 100;
        private static ContinentMap continent;
        
        public static void Load(ContinentMap map)
        {
            continent = map;
        }

        public static void GetRegion(int x, int y, Action<bool, RegionMap> callback)
        {
            if (continent == null)
            {
                Debug.LogError("Continent isn't loaded!");
                callback(false, null);
                return;
            }

            if (x < 0 || y < 0 || x >= continent.regions.GetLength(0) || y >= continent.regions.GetLength(1))
            {
                // Out of bounds.
                callback(false, null);
                return;
            }
            
            if (continent.regions[x, y] != null)
            {
                callback(true, continent.regions[x, y]);
            }
            else
            {
                // This region hasn't been generated yet. We'll do the honors.
                WorldMapGenerator.StartGeneration(RegionSize, RegionSize, Time.time, HandleGenerationComplete, GlobalCoroutineObject.Instance);

                void HandleGenerationComplete(bool success, RegionMap map)
                {
                    if (!success) Debug.LogError("Region generation failed!");
                    continent.regions[x, y] = map;
                    callback(true, map);
                }
            }
        }
    }
}