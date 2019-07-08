using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
	[SerializeField] private GameObject mainMenuCanvas;
	[SerializeField] private GameObject newWorldCanvas;
	[SerializeField] private GameObject characterCreationCanvas;
	
	void Start ()
	{
		SwitchToMainMenuScreen();
	}

	public void OnStartButton ()
	{
		SwitchToCharacterCreationScreen();
	}
	public void OnNewWorldButton ()
	{
		SwitchToNewWorldScreen();
	}
	public void OnSettingsButton ()
	{
		// settings menu
		NotificationManager.Notify("settings?");
	}

	public void SwitchToMainMenuScreen ()
	{
		mainMenuCanvas.SetActive(true);
		newWorldCanvas.SetActive(false);
		characterCreationCanvas.SetActive(false);
	}
	public void SwitchToCharacterCreationScreen ()
	{
		mainMenuCanvas.SetActive(false);
		newWorldCanvas.SetActive(false);
		characterCreationCanvas.SetActive(true);
	}
	public void SwitchToNewWorldScreen()
	{
		mainMenuCanvas.SetActive(false);
		newWorldCanvas.SetActive(true);
		characterCreationCanvas.SetActive(false);
	}
	public void SwitchToSettingsScreen()
	{
		mainMenuCanvas.SetActive(false);
		newWorldCanvas.SetActive(false);
		characterCreationCanvas.SetActive(false);
		// settings canvas active
	}
}
