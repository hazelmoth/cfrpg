using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Stores references to all existing scene portals
public class ScenePortalLibrary : MonoBehaviour
{
	static List<ScenePortal> library;
	public static List<ScenePortal> Library {get{return library;}}

	public static void BuildLibrary () {
		library = new List<ScenePortal> ();
		foreach (ScenePortal portal in GameObject.FindObjectsOfType<ScenePortal>()) {
			library.Add (portal);
		}
	}
	public static List<ScenePortal> GetPortalsBetweenScenes (string startScene, string destScene) {
		List<ScenePortal> results = new List<ScenePortal> ();
		foreach (ScenePortal portal in library) {
			if (portal.gameObject.scene.name == startScene && portal.DestinationScenePrefabId == destScene) {
				results.Add(portal);
			}
		}
		return results;
	}
}
