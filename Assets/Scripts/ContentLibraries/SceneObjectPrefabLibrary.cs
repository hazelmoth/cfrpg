using System.Collections.Generic;
using UnityEngine;

namespace ContentLibraries
{
	[CreateAssetMenu]
	public class SceneObjectPrefabLibrary : ScriptableObject
	{
		[SerializeField] private List<Scene> library = new List<Scene> ();


		[System.Serializable]
		private struct Scene {
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
}
