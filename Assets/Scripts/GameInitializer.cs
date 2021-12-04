using UnityEngine;
using Random = UnityEngine.Random;

/// Manages loading the save and setting up the world.
public class GameInitializer : MonoBehaviour
{
	[SerializeField] private GameObject cameraRigPrefab;

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
				Debug.LogError("Failed to load save " + SaveInfo.SaveToLoad.saveFileId + "! Booting to menu.");
				SceneChangeActivator.GoToMainMenu();
				throw;
			}
		}
	}

    private void AfterSaveLoaded () 
    {
	    isNewWorld = SaveInfo.SaveToLoad.newlyCreated;

	    // If the world is newly generated, we create a save file ID.
	    if (SaveInfo.SaveFileId == null)
	    {
		    Debug.Assert(isNewWorld, "Save file id is null but world is not new!");
		    // TODO this should check that we're not overwriting a file with the same name?
		    SaveInfo.SaveFileId = GeneratedWorldSettings.worldName;
		    if (SaveInfo.SaveFileId == null)
		    {
			    Debug.LogError("Generated world has no name!");
			    SaveInfo.SaveFileId = "MissingName";
		    }
	    }

		if (isNewWorld)
		{
			NewWorldSetup.PerformSetup(cameraRigPrefab);
		}

        // Save the game. This is technically redundant except for newly created worlds.
        GameSaver.SaveGame(SaveInfo.SaveFileId);
		
		InitializationFinished = true;
		PauseManager.Unpause();
	}
}
