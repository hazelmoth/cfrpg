using System.Collections.Generic;
using ContentLibraries;
using UnityEngine;
using UnityEngine.SceneManagement;

// Stores references to all the scene objects currently loaded, and facilitates loading
// of new ones and destruction of old ones
public static class SceneObjectManager
{
	public delegate void SceneLoadedEvent();
	public static event SceneLoadedEvent OnAnySceneLoaded;

	private static SceneObjectPrefabLibrary prefabLibrary;

	// maps scene IDs to scene root objects in the scene
	private static Dictionary<string, GameObject> sceneDict;

	private static int numberOfScenesLoaded = 0;
	private static bool hasInitialized = false;

	private const string PrefabLibraryAssetName = "ScenePrefabLibrary";
	public const string BlankSceneId = "Blank";
	// The name for the unity scene that will be created to hold everything in the world
	private const string WorldUnitySceneName = "World";
	// The default ID for the main world scene object
	public const string WorldSceneId = "World";

	private const float sceneLoadRadius = 400f;

	// Handles Unity scene being destroyed
	private static void OnSceneExit ()
	{
		Debug.Log("SceneObjectManager handling scene exit.");
		hasInitialized = false;
		OnAnySceneLoaded = null;
		prefabLibrary = null;
		sceneDict = null;
		numberOfScenesLoaded = 0;
	}

	// Loads the prefab library and sets up a new unity scene for world loading
	public static void Initialize () 
	{
		if (hasInitialized)
		{
			Debug.LogWarning("SceneObjectManager has already been initialized.");
			return;
		}

		SceneChangeActivator.OnSceneExit += OnSceneExit;

		prefabLibrary = (SceneObjectPrefabLibrary)Resources.Load (PrefabLibraryAssetName);
		if (prefabLibrary == null) {
			Debug.LogError ("Scene prefab library not found.");
		}
		sceneDict = new Dictionary<string, GameObject> ();

		// Locate or create the unity scene for all the scene objects to be loaded into
		if (SceneManager.GetSceneByName(WorldUnitySceneName).IsValid())
		{
			SceneManager.SetActiveScene(SceneManager.GetSceneByName(WorldUnitySceneName));
			Debug.Log("Successfully found a world scene to load scene objects into.");
		}
		else
		{
			Debug.Log("No world scene found. Creating.");
			SceneManager.SetActiveScene(SceneManager.CreateScene(WorldUnitySceneName));
		}

		hasInitialized = true;
		Debug.Log("SceneObjectManager initialized.");
	}

	public static GameObject GetSceneObjectFromId (string sceneId) {
		if (!hasInitialized)
			Initialize ();
		if (sceneId == null) 
		{
			Debug.LogError("Given scene ID shouldn't be null.");
			return null;
		}
		if (sceneDict.ContainsKey(sceneId)) 
		{
			return sceneDict [sceneId];
		}
		else 
		{
			return null;
		}
	}

	/// Returns the ID of the scene which the provided gameObject is a child of.
	public static string GetSceneIdForObject (GameObject gameObject) {
		if (gameObject == null)
		{
			Debug.LogError("Provided object shouldn't be null.");
			return null;
		}
		if (!hasInitialized)
			Initialize ();

		GameObject root = GetSceneRootForObject (gameObject);
		foreach (string sceneId in sceneDict.Keys) {
			GameObject sceneObject = sceneDict [sceneId];

			if (sceneObject.GetInstanceID() == root.GetInstanceID()) {
				return sceneId;
			}
		}
		Debug.LogError("No scene found for given gameObject: " + gameObject.name);
		return null;
	}

	/// Note: this assumes that scene objects are always in the root of the hierarchy
	private static GameObject GetSceneRootForObject (GameObject gameObject) {
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

	/// Creates a new, blank scene object with the given ID.
	public static void CreateBlankScene(string sceneId)
	{
		if (!hasInitialized)
			Initialize();

		if (sceneDict.ContainsKey(sceneId))
		{
			Debug.LogError("A scene with the given ID already exists!");
			return;
		}

		string newSceneId = sceneId;

		GameObject prefab = prefabLibrary.GetScenePrefabFromId(BlankSceneId);
		GameObject newSceneObject = LoadInSceneObject(prefab);
		newSceneObject.name = newSceneId;
		sceneDict.Add(newSceneId, newSceneObject);
		TilemapLibrary.BuildLibrary();
		OnAnySceneLoaded?.Invoke();
	}

	/// Creates a new scene object from the prefab with the given ID.
	/// Adds the new scene to the current region map.
	/// Returns the scene ID for the newly created scene object.
	public static string CreateNewSceneFromPrefab (string scenePrefabId) {
		Debug.Log($"Creating scene from prefab with id {scenePrefabId}");
		if (!hasInitialized)
			Initialize ();
		
		GameObject prefab = prefabLibrary.GetScenePrefabFromId (scenePrefabId);
		if (prefab == null) {
			Debug.LogWarning ("Attempted to create scene from nonexistent prefab ID \"" + scenePrefabId + "\".");
			return null;
		}
		string newSceneId = GetNextAvailableId (scenePrefabId);
		
		// Don't generate a new ID for the world scene; that's a constant
		if (scenePrefabId == WorldSceneId) {
			newSceneId = WorldSceneId;
		}
		GameObject newSceneObject = LoadInSceneObject (prefab);
		newSceneObject.name = newSceneId;
		sceneDict.Add (newSceneId, newSceneObject);
		RegionMapManager.BuildMapForScene(newSceneId, newSceneObject);
		TilemapLibrary.BuildLibrary();
		OnAnySceneLoaded?.Invoke();
		return newSceneId;
	}

	public static void DestroyAllScenes ()
	{
		string[] tempKeyList = new string[sceneDict.Keys.Count];
		sceneDict.Keys.CopyTo(tempKeyList, 0);

		foreach (string scene in tempKeyList)
		{
			DestroyScene(scene);
		}
	}

	// Destroys the scene object with the given ID and removes it from the dictionary
	public static void DestroyScene (string sceneId) {
		Debug.Log("Destroying " + sceneId);
		GameObject.DestroyImmediate(sceneDict[sceneId]);
		sceneDict.Remove(sceneId);
	}

	// Loads the actual scene game object
	private static GameObject LoadInSceneObject(GameObject sceneObjectPrefab)
	{
		if (!hasInitialized)
			Initialize();

		GameObject newSceneObject = GameObject.Instantiate(sceneObjectPrefab);
		newSceneObject.transform.position = GetNextSceneLoadPosition();
		numberOfScenesLoaded++;
		return newSceneObject;
	}

	private static Vector2 GetNextSceneLoadPosition () {
		if (!hasInitialized)
			Initialize ();
		
		// Load scenes in circles of 6 scenes each
		float radius = ((numberOfScenesLoaded + 5f) / 6) * sceneLoadRadius;
		int rotIndex = numberOfScenesLoaded % 6;

		float newX = Mathf.Cos (1f / 3f * Mathf.PI * (float)rotIndex) * radius;
		float newY = Mathf.Sin (1f / 3f * Mathf.PI * (float)rotIndex) * radius;
		newX = Mathf.Floor(newX);
		newY = Mathf.Floor(newY);

		return new Vector2 (newX, newY);
	}

	private static string GetNextAvailableId (string baseId) {
		if (!hasInitialized)
			Initialize ();
		
		if (!sceneDict.ContainsKey(baseId))
		{
			return baseId;
		}
		int currentNum = 2;
		string newId = baseId + "_" + currentNum;
		while (sceneDict.ContainsKey(newId)) {
			currentNum++;
			newId = baseId + "_" + currentNum;
		}
		return newId;
	}
}
