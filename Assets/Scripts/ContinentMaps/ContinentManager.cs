using System;
using UnityEngine;

namespace ContinentMaps
{
    // Stores the currently-loaded Continent Map, allowing the client
    // to get particular region maps, generating them as needed.
    public static class ContinentManager
    {
        public const int RegionSize = 45;
        private static ContinentMap continent;
        
        // Stores the given continent map. Doesn't load any regions.
        public static void Load(ContinentMap map)
        {
            continent = map;
        }

        // Returns the currently loaded continent map.
        public static ContinentMap LoadedMap => continent;

        // Returns a new serializable map with all the data of the current map.
        public static SerializableContinentMap GetSaveData()
        {
            return continent.ToSerializable();
        }

        // Returns the region map at the given coordinates of the continent map. If it hasn't been generated yet, generates
        // it and adds it to the loaded continent map. Calls back with true and the retrieved map if the coordinates are
        // valid and getting the map is successful; calls back false otherwise.
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
                Debug.LogError($"Tried to retrieve an out-of-bounds region ({x}, {y}).\n Check that a region exists before loading it.");
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
                RegionGenerator.StartGeneration(RegionSize, RegionSize, continent.regionInfo[x, y], HandleGenerationComplete, GlobalCoroutineObject.Instance);

                void HandleGenerationComplete(bool success, RegionMap map)
                {
                    if (!success) Debug.LogError("Region generation failed!");
                    continent.regions[x, y] = map;
                    callback(true, map);
                }
            }
        }

        // Stores the given region map at the different coords in the current continent map.
        public static void SaveRegion(RegionMap regionMap, Vector2Int regionCoords)
        {
            if (continent == null)
            {
                Debug.LogError("Continent isn't loaded!");
                return;
            }

            continent.regions[regionCoords.x, regionCoords.y] = regionMap;
        }
    }
}