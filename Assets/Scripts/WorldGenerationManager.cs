using System;
using System.Collections.Generic;
using MyBox;
using ContinentMaps;
using UnityEngine;
using UnityEngine.SceneManagement;

/// Generates a continent and a region of that continent on Start, and
/// load the game scene when it finishes.
public class WorldGenerationManager : MonoBehaviour
{
    private const ulong StartTime = 33750; // 7:30am on the first day
    private const string StartBiome = "heartlands";
    private const int RegionSizeX = ContinentManager.DefaultRegionSize;
    private const int RegionSizeY = ContinentManager.DefaultRegionSize;

    [SerializeField] [MustBeAssigned] private WorldGenerator worldGenerator;

    // Start is called before the first frame update
    private void Start()
    {
        Debug.Assert(worldGenerator != null, "World generator not assigned!");

        WorldMap world;
        try
        {
            world = worldGenerator.Generate(DateTime.Now.Millisecond);
            ContinentManager.Load(world);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            OnGenerationComplete(false, null);
            return;
        }

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
        List<SavedActor> actors = new List<SavedActor>();

        string startRegionId = ChooseStartRegion(map);

        // Enforce that start region must be land
        map.Get(startRegionId).info.isWater = false;
        // Set the region at start coordinates as the player home
        map.Get(startRegionId).info.playerHome = true;
        map.Get(startRegionId).info.feature = null;

		// Make a world save
		WorldSave saveToLoad = new WorldSave(
            worldName: worldName,
            time: StartTime,
            playerActorId: null,
            eventLog: null,
            regionSize: regionSize.ToSerializable(),
            worldMap: map.ToSerializable(),
            currentRegionId: startRegionId,
            actors: actors,
            newlyCreated: true);
        
        
		SaveInfo.SaveToLoad = saveToLoad;
        SaveInfo.RegionSize = regionSize;
        // Here we go boys
        SceneManager.LoadScene((int)UnityScenes.Main);
    }

    private static string ChooseStartRegion(WorldMap map)
    {
        return "town";

        // Try random coordinates until we find a region of the correct biome
        for (int i = 0; i < 100; i++)
        {
            RegionInfo region = map.regions.PickRandom().info;
            if (!region.isWater && region.biome == StartBiome)
            {
                return region.Id;
            }
        }
        Debug.LogWarning("Failed to find suitable start region.");
        return map.regions.PickRandom().Id;
    }
}
