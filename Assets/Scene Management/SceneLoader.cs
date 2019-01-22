using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Responsible for loading in all the interior scenes at the start of the game
public class SceneLoader : MonoBehaviour
{
	public delegate void SceneLoadedEvent();
	public static event SceneLoadedEvent OnScenesLoaded;
	static string ManagerSceneName = "Main";
    // Start is called before the first frame update
    void Start()
    {
		StartCoroutine ("LoadScenes");
    }

	IEnumerator LoadScenes () {
		float rotIndex = 0;
		const float radius = 100;
		for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i ++) {
			Scene scene = SceneManager.GetSceneByBuildIndex (i);
			if (scene.name != ManagerSceneName && !scene.isLoaded) {
				SceneManager.LoadScene(i, LoadSceneMode.Additive);
				// Relocate the scene after loading it, since the original unloaded scene is invalid for some reason
				for (int j = 0; j < SceneManager.sceneCount; j++) {
					if (SceneManager.GetSceneAt (j).buildIndex == i) {
						scene = SceneManager.GetSceneAt (j);
					}
				}
				// It apparently takes 2 frames for the scene to load up
				yield return null;
				yield return null;

				float newX = Mathf.Cos (1f / 3f * Mathf.PI * rotIndex) * radius;
				float newY = Mathf.Sin (1f / 3f * Mathf.PI * rotIndex) * radius;
				scene.GetRootGameObjects()[0].transform.position = new Vector2 (newX, newY);
				Debug.Log (rotIndex);
				rotIndex++;
				Debug.Log (rotIndex);
			}
		}
		if (OnScenesLoaded != null) {
			OnScenesLoaded ();
			Debug.Log ("scenes loaded");
		}
	}
}
