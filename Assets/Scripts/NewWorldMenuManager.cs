using GUI;
using UnityEngine;

public class NewWorldMenuManager : MonoBehaviour
{
	[SerializeField] private MainMenuManager menuManager = null;

    // Start is called before the first frame update
    private void Start()
    {
        if (menuManager == null)
		{
			Debug.LogError("MainMenuManager not assigned");
		}
    }

	public void OnBackButton()
	{
		menuManager.SwitchToMainMenuScreen();
	}
	public void OnFinishButton()
	{
        GeneratedWorldSettings.worldSaveId = "new_world";
        GeneratedWorldSettings.worldName = "New world";
		menuManager.SwitchToCharacterCreationScreen();
	}
}
