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
        GroundMaterialLibrary.LoadLibrary();
        WorldMapGenerator.StartGeneration(sizeX, sizeY, Random.value * 1000, OnGenerationComplete, this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnGenerationComplete (WorldMap map)
    {
        GameDataMaster.SetWorldMap(map);
        SceneManager.LoadScene(1);
    }
}
