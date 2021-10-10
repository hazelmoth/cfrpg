using System;
using System.Collections.Generic;
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

        /// Returns the region map at the given coordinates of the continent map. If it
        /// hasn't been generated yet, generates it and adds it to the loaded continent
        /// map. Calls back with true and the retrieved map if the coordinates are valid
        /// and getting the map is successful; calls back false otherwise.
        public static void GetRegion(string id, Action<bool, RegionMap> callback)
        {
            if (world == null)
            {
                Debug.LogError("Continent isn't loaded!");
                callback(false, null);
                return;
            }

            if (!world.Contains(id))
            {
                // Out of bounds.
                Debug.LogError(
                    $"Tried to retrieve a nonexistent region {id}.\n Check that a region exists before loading it.");
                callback(false, null);
                return;
            }
            
            if (world.Get(id).regionData != null)
            {
                // Place unspawned actors in region
                AddActorsToRegion(world.Get(id).unspawnedActors, world.Get(id).regionData);
                world.Get(id).unspawnedActors.Clear();

                callback(true, world.Get(id).regionData);
            }
            else
            {
                // This region hasn't been generated yet. We'll do the honors.
                Debug.Log($"Generating region {id}");
                RegionGenerator.StartGeneration(
                    DefaultRegionSize,
                    DefaultRegionSize,
                    world.Get(id),
                    HandleGenerationComplete);

                void HandleGenerationComplete(bool success, RegionMap map)
                {
                    if (!success) Debug.LogError("Region generation failed!");
                    world.Get(id).regionData = map;
                    // Place unspawned actors in region
                    AddActorsToRegion(world.Get(id).unspawnedActors, world.Get(id).regionData);
                    world.Get(id).unspawnedActors.Clear();

                    callback(true, world.Get(id).regionData);
                }
            }
        }

        /// Stores the given region map at the provided coords in the current continent map.
        public static void SaveRegion(RegionMap regionMap, string regionId)
        {
            if (world == null)
            {
                Debug.LogError("Continent isn't loaded!");
                return;
            }

            world.Get(regionId).regionData = regionMap;
        }

        private static void AddActorsToRegion(List<string> actorIds, RegionMap regionMap)
        {
            actorIds.ForEach(
                actor =>
                {
                    Vector2 spawnPoint = ActorSpawnpointFinder.FindSpawnPoint(regionMap, SceneObjectManager.WorldSceneId);
                    Location spawnLocation = new Location(spawnPoint, SceneObjectManager.WorldSceneId);
                    regionMap.actors.Add(actor, new RegionMap.ActorPosition(spawnLocation, Direction.Down));
                });
        }

        public static RegionInfo CurrentRegion => LoadedMap.Get(CurrentRegionId);

        /// The region in which the player is currently located.
        public static string CurrentRegionId { get; set; }
    }
}
