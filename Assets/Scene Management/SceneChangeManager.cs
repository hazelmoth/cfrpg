using UnityEngine.SceneManagement;

public class SceneChangeManager
{
	public delegate void SceneExitEvent();
	public static event SceneExitEvent OnSceneExit;

    public static void GoToMainMenu ()
	{
		OnSceneExit?.Invoke();
		// reset the event subscriptions, since some might come from objects that will be destroyed
		OnSceneExit = null;
		SceneManager.LoadScene(0);
	}
}
