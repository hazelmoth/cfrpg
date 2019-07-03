using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Stores references to all the scene objects currently loaded, and facilitates loading
// of new ones and destruction of old ones
public static class SceneObjectManager
{
	public delegate void SceneLoadedEvent();
	public static event SceneLoadedEvent OnInitialScenesLoaded;
	public static event SceneLoadedEvent OnAnySceneLoaded;

	static SceneObjectPrefabLibrary prefabLibrary;

	// maps scene IDs to scene root objects in the scene
	static Dictionary<string, GameObject> sceneDict;

	static int numberOfScenesLoaded = 0;
	static bool hasInitialized = false;
	const string PrefabLibraryAssetName = "ScenePrefabLibrary";
	// The name for the unity scene that will be created to hold everything in the world
	const string WorldUnitySceneName = "World";
	// The default ID for the main world scene object
	public const string WorldSceneId = "World";

	const float sceneLoadRadius = 400f;

	static void OnSceneExit ()
	{
		hasInitialized = false;
		OnInitialScenesLoaded = null;
		OnAnySceneLoaded = null;
		prefabLibrary = null;
		sceneDict = null;
		numberOfScenesLoaded = 0;
	}

	// Loads the prefab library and sets up a new unity scene for world loading
	public static void Initialize () 
	{
		if (hasInitialized)
			return;

		SceneChangeManager.OnSceneExit += OnSceneExit;

		prefabLibrary = (SceneObjectPrefabLibrary)Resources.Load (PrefabLibraryAssetName);
		if (prefabLibrary == null) {
			Debug.LogError ("Scene prefab library not found.");
		}
		sceneDict = new Dictionary<string, GameObject> ();

		// Create the scene for all the scene objects to be loaded into
		if (SceneManager.GetSceneByName(WorldUnitySceneName).IsValid())
		{
			SceneManager.SetActiveScene(SceneManager.GetSceneByName(WorldUnitySceneName));
		}
		else
		{
			SceneManager.SetActiveScene(SceneManager.CreateScene(WorldUnitySceneName));
		}

		hasInitialized = true;
		Debug.Log("SceneObjectManager initialized.");
	}

	public static GameObject GetSceneObjectFromId (string sceneId) {
		if (!hasInitialized)
			Initialize ();
		if (sceneId == null) {
			return null;
		}
		if (sceneDict.ContainsKey(sceneId)) {
			return sceneDict [sceneId];
		}
		else {
			return null;
		}
	}
	public static string GetSceneIdForObject (GameObject gameObject) {
		if (!hasInitialized)
			Initialize ();

		GameObject root = GetSceneRootForObject (gameObject);
		foreach (string sceneId in sceneDict.Keys) {
			GameObject sceneObject = sceneDict [sceneId];
			if (sceneObject.GetInstanceID() == root.GetInstanceID()) {
				return  sceneId;
			}
		}
		return null;
	}
	// This assumes that scene objects are always in the root of the hierarchy
	public static GameObject GetSceneRootForObject (GameObject gameObject) {
		return gameObject.transform.root.gameObject;
	}
	public static bool SceneExists (string sceneId) {
		if (sceneId == null)
			return false;
		if (!hasInitialized)
			Initialize ();
		
		if (sceneDict.ContainsKey(sceneId)) {
			return true;
		}
		return false;
	}

	/// <summary>Creates a new scene object from the prefab with the given ID.</summary>
	/// <returns>The scene ID for the newly created scene object.</returns>
	public static string CreateNewScene (string scenePrefabId) {
		if (!hasInitialized)
			Initialize ();
		
		GameObject prefab = prefabLibrary.GetScenePrefabFromId (scenePrefabId);
		if (prefab == null) {
			Debug.LogWarning ("Attempted to create scene from nonexistent prefab ID.");
			return null;
		}
		string newSceneId = GetNextAvailableId (scenePrefabId);
		// Don't generate a new ID for the world scene, so it's always "World"
		if (scenePrefabId == WorldSceneId) {
			newSceneId = WorldSceneId;
		}
		GameObject newSceneObject = LoadInSceneObject (prefab);
		newSceneObject.name = newSceneId;
		sceneDict.Add (newSceneId, newSceneObject);

		OnAnySceneLoaded?.Invoke();
		return newSceneId;
	}
		
	// Loads the actual scene gameObject
	static GameObject LoadInSceneObject (GameObject sceneObjectPrefab) {
		if (!hasInitialized)
			Initialize ();
		
		GameObject newSceneObject = GameObject.Instantiate (sceneObjectPrefab);
		newSceneObject.transform.position = GetNextSceneLoadPosition();
		numberOfScenesLoaded++;
		return newSceneObject;
	}

	public static void DestroyScene (string sceneId) {
		//TODO destroy scene objects
	}

	static Vector2 GetNextSceneLoadPosition () {
		if (!hasInitialized)
			Initialize ();
		
		// Load scenes in circles of 6 scenes each
		float radius = ((numberOfScenesLoaded + 5) / 6) * sceneLoadRadius;
		int rotIndex = numberOfScenesLoaded % 6;

		float newX = Mathf.Cos (1f / 3f * Mathf.PI * (float)rotIndex) * radius;
		float newY = Mathf.Sin (1f / 3f * Mathf.PI * (float)rotIndex) * radius;

		return new Vector2 (newX, newY);
	}
	static string GetNextAvailableId (string baseId) {
		if (!hasInitialized)
			Initialize ();
		
		int currentNum = 0;
		string newId = baseId + "_" + currentNum;
		while (sceneDict.ContainsKey(newId)) {
			currentNum++;
			newId = baseId + "_" + currentNum;
		}
		return newId;
	}
}
