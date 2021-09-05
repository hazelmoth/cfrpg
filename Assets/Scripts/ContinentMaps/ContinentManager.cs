using System;
using UnityEngine;

namespace ContinentMaps
{
    /// Stores the currently-loaded Continent Map, allowing the client
    /// to get particular region maps, generating them as needed.
    public static class ContinentManager
    {
        public const int DefaultRegionSize = 45;
        private static WorldMap world;
        
        /// Stores the given continent map. Doesn't load any regions.
        public static void Load(WorldMap map)
        {
            world = map;
        }

        /// Returns the currently loaded continent map.
        public static WorldMap LoadedMap => world;

        /// Returns a new serializable map with all the data of the current map.
        public static SerializableWorldMap GetSaveData()
        {
            return world.ToSerializable();
        }

        public static void GetRegion(Vector2Int coords, Action<bool, RegionMap> callback)
        {
            GetRegion(coords.x, coords.y, callback);
        }

        /// Returns the region map at the given coordinates of the continent map. If it
        /// hasn't been generated yet, generates it and adds it to the loaded continent
        /// map. Calls back with true and the retrieved map if the coordinates are valid
        /// and getting the map is successful; calls back false otherwise.
        public static void GetRegion(int x, int y, Action<bool, RegionMap> callback)
        {
            if (world == null)
            {
                Debug.LogError("Continent isn't loaded!");
                callback(false, null);
                return;
            }

            if (x < 0 || y < 0 || x >= world.regions.GetLength(0) || y >= world.regions.GetLength(1))
            {
                // Out of bounds.
                Debug.LogError($"Tried to retrieve an out-of-bounds region ({x}, {y}).\n Check that a region exists before loading it.");
                callback(false, null);
                return;
            }
            
            if (world.regions[x, y] != null)
            {
                callback(true, world.regions[x, y]);
            }
            else
            {
                // This region hasn't been generated yet. We'll do the honors.
                Debug.Log($"Generating region {x}, {y}");
                RegionGenerator.StartGeneration(
                    DefaultRegionSize,
                    DefaultRegionSize,
                    world.regionInfo[x, y],
                    HandleGenerationComplete);

                void HandleGenerationComplete(bool success, RegionMap map)
                {
                    if (!success) Debug.LogError("Region generation failed!");
                    world.regions[x, y] = map;
                    callback(true, map);
                }
            }
        }

        /// Stores the given region map at the provided coords in the current continent map.
        public static void SaveRegion(RegionMap regionMap, Vector2Int regionCoords)
        {
            if (world == null)
            {
                Debug.LogError("Continent isn't loaded!");
                return;
            }

            world.regions[regionCoords.x, regionCoords.y] = regionMap;
        }
    }
}