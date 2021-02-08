using UnityEngine;

namespace GUI
{

	// Responsible for activating the pause menu and managing its functions, and pausing the game
	public class PauseMenuManager : MonoBehaviour
	{
		[SerializeField] private Canvas pauseMenuCanvas = null;
		private bool menuIsActive = false;
		// Start is called before the first frame update
		private void Start()
		{
			SetPauseMenuActive(false);
			KeyInputHandler.OnPauseButton += OnPauseButton;
		}

		private void OnPauseButton()
		{
			if (menuIsActive)
			{
				SetPauseMenuActive(false);
				PauseManager.Unpause();
			}
			else
			{
				SetPauseMenuActive(true);
				PauseManager.Pause();
			}
		}

		private void SetPauseMenuActive(bool active)
		{
			pauseMenuCanvas.enabled = active;
			menuIsActive = active;
		}
		// Pause menu buttons
		public void OnMainMenuButton()
		{
			// TODO double check with the user
			PauseManager.Unpause();
			GameSaver.SaveGame(SaveInfo.SaveFileId);
			SceneChangeActivator.GoToMainMenu();
		}
		public void OnSettingsButton()
		{
			//TODO...settings screen
		}
	}
}