using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SceneObjectPrefabLibrary : ScriptableObject
{
	[SerializeField]
	List<Scene> library = new List<Scene> ();


	[System.Serializable]
	struct Scene {
		public string prefabId;
		public GameObject scenePrefab;
	}

	public GameObject GetScenePrefabFromId (string prefabId) {
		foreach (Scene scene in library) {
			if (scene.prefabId == prefabId) {
				return scene.scenePrefab;
			}
		}
		return null;
	}
}
