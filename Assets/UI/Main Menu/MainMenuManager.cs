using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
	[SerializeField] private GameObject mainMenuCanvas;
	[SerializeField] private GameObject newWorldCanvas;
	
	void Start ()
	{
		SwitchToMainMenuScreen();
	}

	public void OnStartButton ()
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
	}
	public void SwitchToNewWorldScreen()
	{
		mainMenuCanvas.SetActive(false);
		newWorldCanvas.SetActive(true);
	}
	public void SwitchToSettingsScreen()
	{
		mainMenuCanvas.SetActive(false);
		newWorldCanvas.SetActive(false);
		// settings canvas active
	}
}
