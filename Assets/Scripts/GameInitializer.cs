using UnityEngine;
using Random = UnityEngine.Random;

/// Manages loading the save and setting up the world.
public class GameInitializer : MonoBehaviour
{
	public static bool InitializationFinished { get; private set; }

	private bool isNewWorld;

    // Start is called before the first frame update
    private void Start()
	{
		InitializationFinished = false;
		PauseManager.Pause();
		Console.Initialize();
		SceneObjectManager.Initialize ();
		Random.InitState((int)System.DateTime.Now.Ticks);

		if (SaveInfo.SaveToLoad == null)
		{
			Debug.LogError("Main scene started with no save ready to load! Booting to menu.");
			SceneChangeActivator.GoToMainMenu();
		}
		else
		{
			try
			{
				SaveLoader.LoadSave(SaveInfo.SaveToLoad, AfterSaveLoaded);
			}
			catch
			{
				SceneChangeActivator.GoToMainMenu();
				throw;
			}
		}
	}

    private void AfterSaveLoaded () 
    {
	    isNewWorld = SaveInfo.SaveToLoad.newlyCreated;

		if (isNewWorld)
		{
			NewWorldSetup.PerformSetup();
		}

        //TEST
        GameSaver.SaveGame(SaveInfo.SaveFileId);
		
		InitializationFinished = true;
		PauseManager.Unpause();
	}
}