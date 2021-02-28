using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

// Called in a scene by itself immediately after the game is launched;
// for setting up data needed before the main menu
public class GameLaunchManager : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI statusText;
	
	// Start is called before the first frame update
	private void Start()
	{
		statusText.text = "Loading assets...";
		try
		{
			ContentLibrary.Instance.LoadAllLibraries();
			SceneManager.LoadScene((int) UnityScenes.Menu);
		}
		catch (Exception ex)
		{
			statusText.text = "EXCEPTION: " + ex.Message + "\n\n" + ex.StackTrace;
			throw;
		}
	}
}
