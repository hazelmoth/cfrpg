using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
	[SerializeField] private GameObject mainMenuCanvas;
	[SerializeField] private GameObject newWorldCanvas;
	[SerializeField] private GameObject worldSelectCanvas;
	[SerializeField] private GameObject characterCreationCanvas;
	
	void Start ()
	{
		SwitchToCanvas(mainMenuCanvas);
	}

	public void OnStartButton ()
	{
		SwitchToCanvas(characterCreationCanvas);
	}
	public void OnLoadWorldButton ()
	{
		SwitchToLoadWorldScreen();
	}
	public void OnNewWorldButton ()
	{
		SwitchToCanvas(newWorldCanvas);
	}
	public void OnSettingsButton ()
	{
		// settings menu
		NotificationManager.Notify("settings not implemented");
	}

	public void SwitchToLoadWorldScreen ()
	{
		LoadWorldMenuManager worldSelectUiManager = FindObjectOfType<LoadWorldMenuManager>();
		worldSelectUiManager.PopulateWorldList();
		SwitchToCanvas(worldSelectCanvas);
	}
	public void SwitchToMainMenuScreen()
	{
		SwitchToCanvas(mainMenuCanvas);
	}
	public void SwitchToNewWorldScreen()
	{
		SwitchToCanvas(newWorldCanvas);
	}
	public void SwitchToCharacterCreationScreen()
	{
		SwitchToCanvas(characterCreationCanvas);
	}

	void SwitchToCanvas (GameObject canvas)
	{
		mainMenuCanvas.SetActive(false);
		newWorldCanvas.SetActive(false);
		worldSelectCanvas.SetActive(false);
		characterCreationCanvas.SetActive(false);

		canvas.SetActive(true);
	}
}
