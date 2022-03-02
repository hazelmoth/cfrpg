using System;
using System.Collections.Generic;
using System.Linq;
using MyBox;
using ContinentMaps;
using SettlementSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

/// Generates a continent and a region of that continent on Start, and
/// load the game scene when it finishes.
public class WorldGenerationManager : MonoBehaviour
{
    private const int RegionSizeX = ContinentManager.DefaultRegionSize;
    private const int RegionSizeY = ContinentManager.DefaultRegionSize;

    /// 7:30 am on the first day of week 2. We skip a week to allow plants to grow.
    private static readonly ulong StartTime = (ulong) (TimeKeeper.TicksPerInGameDay * 7.3125);

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
            settlements: new Dictionary<string, SettlementManager.SettlementInfo>(),
            newlyCreated: true);
        
        
		SaveInfo.SaveToLoad = saveToLoad;
        SaveInfo.RegionSize = regionSize;
        // Here we go boys
        SceneManager.LoadScene((int)UnityScenes.Main);
    }

    private static string ChooseStartRegion(WorldMap map)
    {
        // TODO allow WorldMap to specify a start region
        return map.regions.Any(r => r.Id == "town")
            ? "town"
            : map.regions.First().Id;
    }
}
