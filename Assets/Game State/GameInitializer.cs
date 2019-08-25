using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Manages the whole launch sequence (scene loading, asset loading, etc)
public class GameInitializer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
	{
		EntityLibrary.LoadLibrary ();
		GroundMaterialLibrary.LoadLibrary ();
		SceneObjectManager.Initialize ();
		Random.InitState((int)System.DateTime.Now.Ticks);

		if (GameDataMaster.SaveToLoad == null)
		{
			Debug.LogError("Scene started with no save loaded!");
		}
		else
		{
			SaveLoader.LoadSave(GameDataMaster.SaveToLoad, AfterSaveLoaded);
		}
		
	}
	void AfterSaveLoaded () {

		// TODO - Load any mod assets
		PlayerSpawner.Spawn(SceneObjectManager.WorldSceneId, ActorSpawnpointFinder.FindSpawnPointNearCoords(SceneObjectManager.WorldSceneId, new Vector2(100, 100)));


        // TEST obviously temporary
        PlayerDucats.SetDucatBalance (666);

		//TEST
		DroppedItemSpawner.SpawnItem("log", new Vector2(10, 10), SceneObjectManager.WorldSceneId, true);

		NotificationManager.Notify ("We're go.");

        //TEST
        GameSaver.SaveGame();
	}
}

