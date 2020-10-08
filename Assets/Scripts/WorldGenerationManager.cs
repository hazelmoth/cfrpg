using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldGenerationManager : MonoBehaviour
{
	private int sizeX = 100;
	private int sizeY = 100;

    // Start is called before the first frame update
    private void Start()
    {
        WorldMapGenerator.StartGeneration(sizeX, sizeY, Random.value * 1000, OnGenerationComplete, this);
    }

    private void OnGenerationComplete (WorldMap map)
    {
		string worldName = GeneratedWorldSettings.worldName;
        Vector2Int worldSize = new Vector2Int(sizeX, sizeY);
        ulong time = 0;
		// Make an otherwise blank world save with this map
		WorldSave saveToLoad = new WorldSave(worldName, time, new SerializableWorldMap(map), worldSize.ToSerializable(), null, new List<SavedEntity>(), new List<SavedActor>(), null, new List<SerializableScenePortal>(), true);
		GameDataMaster.SaveToLoad = saveToLoad;
        GameDataMaster.WorldSize = worldSize;
        SceneManager.LoadScene((int)UnityScenes.Main);
    }
}
