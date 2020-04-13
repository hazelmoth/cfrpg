using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Manages the whole launch sequence (scene loading, asset loading, etc)
public class GameInitializer : MonoBehaviour
{
	private bool isNewWorld;

    // Start is called before the first frame update
    void Start()
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
			SaveLoader.LoadSave(GameDataMaster.SaveToLoad, GameDataMaster.PlayerToLoad, AfterSaveLoaded);
		}
		
	}
	void AfterSaveLoaded () {

		isNewWorld = GameDataMaster.SaveToLoad.newlyCreated;

		// TEST obviously temporary
		PlayerDucats.SetDucatBalance(666);

		if (isNewWorld)
		{
			NewGameSetup.PerformSetup();
		}


		NotificationManager.Notify ("We're go.");

        //TEST
        GameSaver.SaveGame(GameDataMaster.SaveFileId);
	}
}

