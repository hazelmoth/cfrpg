using System.Collections.Generic;
using UnityEngine.SceneManagement;

// A helper class to free classes from directly interfacing with SceneManagement
public static class SceneFinder
{   
	public static List<string> GetLoadedSceneNames () {
		List<string> scenes = new List<string> ();
		for (int i = 0; i < SceneManager.sceneCount; i++) {
			scenes.Add (SceneManager.GetSceneAt (i).name);
		}
		return scenes;
	}
}
