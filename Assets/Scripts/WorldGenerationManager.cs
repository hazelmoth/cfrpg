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
    private const int SizeX = 100;
    private const int SizeY = 100;
    private const int WorldSizeX = 25;
    private const int WorldSizeY = 25;
    
    // Have the player start somewhere in the middle-ish of the world
    private static readonly Vector2IntSerializable StartRegionCoords = new Vector2IntSerializable(10, 10);

    // Start is called before the first frame update
    private void Start()
    {
        // Don't bother generating any regions yet; just generate the world info
        ContinentMap continent = ContinentGenerator.Generate(WorldSizeX, WorldSizeY, DateTime.Now.Millisecond);
        ContinentManager.Load(continent);
        OnGenerationComplete(true, continent);
    }

    private void OnGenerationComplete (bool success, ContinentMap map)
    {
        if (!success)
        {
            Debug.LogError("World generation failed!");
            SceneManager.LoadScene((int)UnityScenes.Menu);
            return;
        }
		string worldName = GeneratedWorldSettings.worldName;
        Vector2Int regionSize = new Vector2Int(SizeX, SizeY);
        ulong time = StartTime;
        List<SavedEntity> entities = new List<SavedEntity>();
        List<SavedActor> actors = new List<SavedActor>();
        List<SavedDroppedItem> items = new List<SavedDroppedItem>();
        List<SerializableScenePortal> scenePortals = new List<SerializableScenePortal>();
        
        // Set the region at start coordinates as the player home
        map.regionInfo[StartRegionCoords.x, StartRegionCoords.y].playerHome = true;

		// Make a world save (without any generated regions yet)
		WorldSave saveToLoad = new WorldSave(
            worldName: worldName,
            time: time,
            playerActorId: null,
            eventLog: null,
            regionSize: regionSize.ToSerializable(),
            continentMap: map.ToSerializable(),
            currentRegionCoords: StartRegionCoords,
            entities: entities,
            actors: actors,
            items: items,
            newlyCreated: true);
        
        
		SaveInfo.SaveToLoad = saveToLoad;
        SaveInfo.RegionSize = regionSize;
        SceneManager.LoadScene((int)UnityScenes.Main);
    }
}
