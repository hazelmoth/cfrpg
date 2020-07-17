using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Manages loading the save and setting up the world
public class GameInitializer : MonoBehaviour
{
	private bool isNewWorld;

    // Start is called before the first frame update
    private void Start()
	{
		Console.Initialize();
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

    private void AfterSaveLoaded () {

		isNewWorld = GameDataMaster.SaveToLoad.newlyCreated;


		if (isNewWorld)
		{
			NewWorldSetup.PerformSetup();
		}


		NotificationManager.Notify ("We're go.");

        //TEST
        GameSaver.SaveGame(GameDataMaster.SaveFileId);
	}
}

