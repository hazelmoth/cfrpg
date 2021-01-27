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

    // Start is called before the first frame update
    private void Start()
    {
        ContinentMap continent = ContinentGenerator.Generate(25, 25, DateTime.Now.Millisecond);
        ContinentManager.Load(continent);
        ContinentManager.GetRegion(10,10,OnGenerationComplete);
    }

    private void OnGenerationComplete (bool success, RegionMap map)
    {
        if (!success)
        {
            Debug.LogError("World generation failed!");
            SceneManager.LoadScene((int)UnityScenes.Menu);
            return;
        }
		string worldName = GeneratedWorldSettings.worldName;
        Vector2Int worldSize = new Vector2Int(SizeX, SizeY);
        ulong time = StartTime;
        List<SavedEntity> entities = new List<SavedEntity>();
        List<SavedActor> actors = new List<SavedActor>();
        List<SavedDroppedItem> items = new List<SavedDroppedItem>();
        List<SerializableScenePortal> scenePortals = new List<SerializableScenePortal>();

		// Make an otherwise blank world save with this map
		WorldSave saveToLoad = new WorldSave(worldName, time, new SerializableWorldMap(map), worldSize.ToSerializable(), null, entities, actors, null, items, scenePortals, true);
		GameDataMaster.SaveToLoad = saveToLoad;
        GameDataMaster.RegionSize = worldSize;
        SceneManager.LoadScene((int)UnityScenes.Main);
    }
}
