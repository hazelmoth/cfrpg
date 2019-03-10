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

	public static void LoadScenes () {
		SceneLoader instance = GameObject.FindObjectOfType<SceneLoader> ();
		IEnumerator coroutine = instance.LoadScenesCoroutine (null);
		instance.StartCoroutine (coroutine);
	}
	public static void LoadScenes(SceneLoadedEvent callback) {
		SceneLoader instance = GameObject.FindObjectOfType<SceneLoader> ();
		IEnumerator coroutine = instance.LoadScenesCoroutine (callback);
		instance.StartCoroutine (coroutine);
	}
	IEnumerator LoadScenesCoroutine (SceneLoadedEvent callback) {
		float rotIndex = 0;
		const float radius = 100;
		for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i ++) {
			Scene scene = SceneManager.GetSceneByBuildIndex (i);
			if (scene.name != ManagerSceneName && !scene.isLoaded) {
				SceneManager.LoadScene(i, LoadSceneMode.Additive);
				// Relocate the scene after loading it, since the original unloaded scene is invalid
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
				rotIndex++;
			}
		}
		if (OnScenesLoaded != null) {
			OnScenesLoaded ();
			Debug.Log ("scenes loaded");
		}
		if (callback != null) {
			callback ();
		}
	}
}
