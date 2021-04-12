using System.Collections;
using ContinentMaps;
using UnityEngine;

public class SaveLoader
{
	public delegate void SaveLoaderCallback();
	public delegate void SaveLoadedEvent();
	public static SaveLoadedEvent OnSaveLoaded;

	public static void LoadSave(WorldSave save, SaveLoaderCallback callback)
    {
		IEnumerator coroutine = LoadSaveCoroutine(save, callback);
		GlobalCoroutineObject.Instance.StartCoroutine(coroutine);
    }

	private static IEnumerator LoadSaveCoroutine(WorldSave save, SaveLoaderCallback callback)
	{
		SaveInfo.WorldName = save.worldName;
		SaveInfo.RegionSize = save.regionSize.ToNonSerializable();
		
		// Register actors
		foreach(SavedActor savedActor in save.actors)
		{
			ActorData data = savedActor.data.ToNonSerializable();
			ActorRegistry.Register(data);
			
			if (save.playerActorId == data.actorId)
			{
				PlayerController.SetPlayerActor(data.actorId);
			}
		}
		

		ContinentManager.Load(save.continentMap.ToNonSerializable());
		
		// Load the current region
		RegionMap regionMap = null;
		bool mapReady = false;
		bool mapLoadSucceeded = false;
		ContinentManager.GetRegion(save.currentRegionCoords.x, save.currentRegionCoords.y,
			(b, map) =>
			{
				regionMap = map;
				mapReady = true;
				mapLoadSucceeded = b;
			});
		// Wait for map loading to finish
		while (!mapReady)
		{
			yield return null;
		}

		if (!mapLoadSucceeded)
		{
			// Map loading failed.
			// TODO catch this properly and throw back to the menu
			Debug.LogError("Failed to load the player's current region!");
			callback?.Invoke();
		}
		
		// Load the region the player is currently in
		RegionMapManager.CurrentRegionCoords = save.currentRegionCoords.ToNonSerializable();
		RegionMapManager.LoadMap(regionMap);

		TimeKeeper.SetCurrentTick(save.time);

		History history = GameObject.FindObjectOfType<History>();
		history.LoadEventLog(save.eventLog);

        foreach (SavedEntity entity in save.entities)
        {
            GameObject entityObject = RegionMapManager.GetEntityObjectAtPoint(entity.location.ToNonSerializable(), entity.scene);
            EntityObject entityObjectObject = entityObject.GetComponent<EntityObject>();
            entityObjectObject.SetStateData(entity);
        }
        ScenePortalLibrary.BuildLibrary();

		if (save.items != null)
		{
			foreach (SavedDroppedItem item in save.items)
			{
				DroppedItemSpawner.SpawnItem(item.item, item.location.ToVector2(), item.scene);
			}
		} else
		{
			Debug.LogWarning("Save is missing dropped items list.");
		}



		OnSaveLoaded?.Invoke();
		callback?.Invoke();
		yield return null;
	}

	// Loads the given scene portal into the currently-loaded region.
	public static void SpawnScenePortal(SerializableScenePortal portalData)
	{
		GameObject newPortalObject = new GameObject("Scene portal");
		if (portalData.portalScene == null)
		{
			Debug.LogError("Saved scene portal has no data for what scene it's in! Not loading this portal.");
			return;
		}
		else if (!SceneObjectManager.SceneExists(portalData.portalScene))
		{
			Debug.LogError("Saved scene portal belongs to a scene \"" + portalData.portalScene + "\" that doesn't currently exist! Not loading this portal.");
			return;
		}
		newPortalObject.transform.SetParent(SceneObjectManager.GetSceneObjectFromId(portalData.portalScene).transform);
		newPortalObject.transform.position = TilemapInterface.ScenePosToWorldPos(portalData.sceneRelativeLocation.ToVector2(), portalData.portalScene);
		ScenePortal newPortal = newPortalObject.AddComponent<ScenePortal>();
		newPortal.SetData(portalData);
		BoxCollider2D portalCollider = newPortalObject.AddComponent<BoxCollider2D>();
		portalCollider.isTrigger = true;
	}
}
