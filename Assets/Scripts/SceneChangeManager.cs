using UnityEngine.SceneManagement;

public class SceneChangeActivator
{
	public delegate void SceneExitEvent();
	public static event SceneExitEvent OnSceneExit;

    public static void GoToMainMenu ()
	{
		OnSceneExit?.Invoke();
		// reset the event subscriptions, since some might come from objects that will be destroyed
		OnSceneExit = null;
		SceneManager.LoadScene((int)UnityScenes.Menu);
	}

    public static void BootToErrorScene(string message, string error)
    {
	    {
		    OnSceneExit?.Invoke();
		    OnSceneExit = null;
		    ErrorSceneManager.SetDisplayedError(message, error);
		    SceneManager.LoadScene((int)UnityScenes.Error);
	    }
    }
}
