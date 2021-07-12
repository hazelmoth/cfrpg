using System;
using System.Collections.Generic;
using ContinentMaps;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

// Generates a continent and a region of that continent on Start, and
// load the game scene when it finishes.
public class WorldGenerationManager : MonoBehaviour
{
    private const ulong StartTime = 33750; // 7:30am on the first day
    private const string StartBiome = "heartlands";
    private const int RegionSizeX = ContinentManager.RegionSize;
    private const int RegionSizeY = ContinentManager.RegionSize;
    private const int WorldSizeX = 15;
    private const int WorldSizeY = 8;

    // Start is called before the first frame update
    private void Start()
    {
        // Don't bother generating any regions yet; just generate the world info
        WorldMap world = ContinentGenerator.Generate(WorldSizeX, WorldSizeY, DateTime.Now.Millisecond);
        ContinentManager.Load(world);
        OnGenerationComplete(true, world);
    }

    private void OnGenerationComplete (bool success, WorldMap map)
    {
        if (!success)
        {
            Debug.LogError("World generation failed!");
            SceneManager.LoadScene((int)UnityScenes.Menu);
            return;
        }
		string worldName = GeneratedWorldSettings.worldName;
        Vector2Int regionSize = new Vector2Int(RegionSizeX, RegionSizeY);
        ulong time = StartTime;
        List<SavedActor> actors = new List<SavedActor>();

        Vector2IntSerializable startRegionCoords = ChooseStartRegion(map);
        
        // Enforce that start region must be land
        map.regionInfo[startRegionCoords.x, startRegionCoords.y].isWater = false;
        // Set the region at start coordinates as the player home
        map.regionInfo[startRegionCoords.x, startRegionCoords.y].playerHome = true;

		// Make a world save (without any generated regions yet)
		WorldSave saveToLoad = new WorldSave(
            worldName: worldName,
            time: time,
            playerActorId: null,
            eventLog: null,
            regionSize: regionSize.ToSerializable(),
            worldMap: map.ToSerializable(),
            currentRegionCoords: startRegionCoords,
            actors: actors,
            newlyCreated: true);
        
        
		SaveInfo.SaveToLoad = saveToLoad;
        SaveInfo.RegionSize = regionSize;
        SceneManager.LoadScene((int)UnityScenes.Main);
    }

    private static Vector2IntSerializable ChooseStartRegion(WorldMap map)
    {
        // Try random coordinates until we find a region of the correct biome
        for (int i = 0; i < 100; i++)
        {
            int x = Random.Range(0, map.dimensions.x);
            int y = Random.Range(0, map.dimensions.y);

            if (!map.regionInfo[x,y].isWater && 
                map.regionInfo[x, y].biome == StartBiome)
            {
                return new Vector2IntSerializable(x, y);
            }
        }
        Debug.LogWarning("Failed to find suitable start region.");
        return new Vector2IntSerializable(5, 5);
    }
}
