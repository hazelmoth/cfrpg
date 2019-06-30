using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Responsible for activating the pause menu and managing its functions, and pausing the game
public class PauseMenuManager : MonoBehaviour
{
	[SerializeField] Canvas pauseMenuCanvas = null;
	bool menuIsActive = false;
    // Start is called before the first frame update
    void Start()
    {
		SetPauseMenuActive (false);
		KeyInputHandler.OnPauseButton += OnPauseButton;
    }
	void OnPauseButton () {
		if (menuIsActive) {
			SetPauseMenuActive (false);
			PauseManager.Unpause ();
		} else {
			SetPauseMenuActive (true);
			PauseManager.Pause ();
		}
	}
	void SetPauseMenuActive (bool active) {
		pauseMenuCanvas.enabled = active;
		menuIsActive = active;
	}
	// Pause menu buttons
	public void OnMainMenuButton ()
	{
		// double check with the user
		// save the game or whatever
		PauseManager.Unpause();
		SceneManager.LoadScene(0);
	}
	public void OnSettingsButton ()
	{
		//...
	}
}
