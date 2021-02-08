using UnityEngine;

// Manages loading the save and setting up the world
public class GameInitializer : MonoBehaviour
{
	public static bool InitializationFinished { get; private set; }

	private bool isNewWorld;

    // Start is called before the first frame update
    private void Start()
	{
		InitializationFinished = false;

		Console.Initialize();
		SceneObjectManager.Initialize ();
		Random.InitState((int)System.DateTime.Now.Ticks);

		if (SaveInfo.SaveToLoad == null)
		{
			Debug.LogError("Scene started with no save loaded!");
		}
		else
		{
			SaveLoader.LoadSave(SaveInfo.SaveToLoad, AfterSaveLoaded);
		}
		
	}

    private void AfterSaveLoaded () {

		isNewWorld = SaveInfo.SaveToLoad.newlyCreated;


		if (isNewWorld)
		{
			NewWorldSetup.PerformSetup();
		}

        //TEST
        GameSaver.SaveGame(SaveInfo.SaveFileId);

		InitializationFinished = true;
	}
}

