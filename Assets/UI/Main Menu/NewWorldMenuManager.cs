using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewWorldMenuManager : MonoBehaviour
{
	[SerializeField] private MainMenuManager menuManager;

    // Start is called before the first frame update
    void Start()
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
	public void OnGenerateButton()
	{
		SceneManager.LoadScene(1, LoadSceneMode.Single);
	}
}
