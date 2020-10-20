using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldGenerationManager : MonoBehaviour
{
    private const ulong startTime = 33750; // 7:30am on the first day
	private int sizeX = 100;
	private int sizeY = 100;

    // Start is called before the first frame update
    private void Start()
    {
        WorldMapGenerator.StartGeneration(sizeX, sizeY, Random.value * 1000, OnGenerationComplete, this);
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
        Vector2Int worldSize = new Vector2Int(sizeX, sizeY);
        ulong time = startTime;
        List<SavedEntity> entities = new List<SavedEntity>();
        List<SavedActor> actors = new List<SavedActor>();
        List<SavedDroppedItem> items = new List<SavedDroppedItem>();
        List<SerializableScenePortal> scenePortals = new List<SerializableScenePortal>();

		// Make an otherwise blank world save with this map
		WorldSave saveToLoad = new WorldSave(worldName, time, new SerializableWorldMap(map), worldSize.ToSerializable(), null, entities, actors, null, items, scenePortals, true);
		GameDataMaster.SaveToLoad = saveToLoad;
        GameDataMaster.WorldSize = worldSize;
        SceneManager.LoadScene((int)UnityScenes.Main);
    }
}
