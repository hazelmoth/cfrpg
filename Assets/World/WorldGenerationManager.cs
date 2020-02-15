using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldGenerationManager : MonoBehaviour
{
    int sizeX = 300;
    int sizeY = 300;

    // Start is called before the first frame update
    void Start()
    {
        WorldMapGenerator.StartGeneration(sizeX, sizeY, Random.value * 1000, OnGenerationComplete, this);
    }

    void OnGenerationComplete (WorldMap map)
    {
		string worldName = GeneratedWorldSettings.worldName;
		// Make an otherwise blank world save with this map
		WorldSave saveToLoad = new WorldSave(worldName, new SerializableWorldMap(map), new List<SavedEntity>(), new List<SavedNpc>(), new List<SerializableScenePortal>());
		GameDataMaster.SaveToLoad = saveToLoad;
        SceneManager.LoadScene((int)UnityScenes.Main);
    }
}
