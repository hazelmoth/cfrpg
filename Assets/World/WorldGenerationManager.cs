using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldGenerationManager : MonoBehaviour
{
    int sizeX = 200;
    int sizeY = 200;

    // Start is called before the first frame update
    void Start()
    {
        WorldMapGenerator.StartGeneration(sizeX, sizeY, Random.value * 1000, OnGenerationComplete, this);
    }

    void OnGenerationComplete (WorldMap map)
    {
		string worldName = GeneratedWorldSettings.worldName;
		WorldSave saveToLoad = new WorldSave(worldName, new SerializableWorldMap(map), new List<SavedEntity>(), new List<SavedNpc>());
		GameDataMaster.SaveToLoad = saveToLoad;
        SceneManager.LoadScene((int)UnityScenes.Main);
    }
}
