using System;
using System.Collections;
using System.Threading.Tasks;
using ContinentMaps;
using SettlementSystem;
using UnityEngine;
using WorldState;
using Newtonsoft.Json;
using Object = UnityEngine.Object;

public class SaveLoader
{
	public delegate void SaveLoadedEvent();
	public static SaveLoadedEvent OnSaveLoaded;
	
	public static async Task Load(WorldSave save, Action callback)
	{
		SaveInfo.WorldName = save.worldName;
		SaveInfo.RegionSize = save.regionSize.ToNonSerializable();

		// Register actors
		foreach(ActorData savedActor in save.actors)
		{
			ActorRegistry.Register(savedActor);
			
			if (save.playerActorId == savedActor.ActorId) PlayerController.SetPlayerActor(savedActor.ActorId);
		}

		// Set world state
		WorldStateManager worldStateManager = GameObject.FindObjectOfType<WorldStateManager>();
		if (worldStateManager != null) worldStateManager.Init(save.worldState);
		else Debug.LogError("Failed to find WorldStateManager object. State won't be loaded.");

		// Load world map
		ContinentManager.Load(save.worldMap.ToNonSerializable());

		// Load the current region
		RegionMap regionMap = null;
		bool mapReady = false;
		bool mapLoadSucceeded = false;
		ContinentManager.GetRegion(save.currentRegionId,
			(b, map) =>
			{
				regionMap = map;
				mapReady = true;
				mapLoadSucceeded = b;
			});
		
		// Wait for map loading to finish
		while (!mapReady)
		{
			await (Task.Delay(25));
		}

		if (!mapLoadSucceeded)
		{
			// Map loading failed.
			throw new Exception("Failed to load player's current region!");
		}

		// Load the region the player is currently in
		ContinentManager.CurrentRegionId = save.currentRegionId;
		RegionMapManager.LoadMap(regionMap);

		TimeKeeper.SetCurrentTick(save.time);

		History history = GameObject.FindObjectOfType<History>();
		history.LoadEventLog(save.eventLog);
		
        ScenePortalLibrary.BuildLibrary();

        if (save.settlements != null)
			Object.FindObjectOfType<SettlementManager>()?.Initialize(save.settlements);
        else
			Debug.LogWarning("No settlement data found in save file.");

        OnSaveLoaded?.Invoke();
		callback?.Invoke();
	}

	// Loads the given scene portal into the currently-loaded region.
	public static void SpawnScenePortal(SerializableScenePortal portalData)
	{
		GameObject newPortalObject = new("Scene portal");
		if (portalData.portalScene == null)
		{
			Debug.LogError(
				"Saved scene portal has no data for what scene it's in! Not loading this portal.");
			return;
		}
		else if (!SceneObjectManager.SceneExists(portalData.portalScene))
		{
			Debug.LogError(
				"Saved scene portal belongs to a scene \""
				+ portalData.portalScene
				+ "\" that doesn't currently exist! Not loading this portal.");
			return;
		}
		newPortalObject.transform.SetParent(
			SceneObjectManager.GetSceneObjectFromId(portalData.portalScene).transform);
		newPortalObject.transform.position = TilemapInterface.ScenePosToWorldPos(
			portalData.sceneRelativeLocation.ToVector2(),
			portalData.portalScene);
		ScenePortal newPortal = newPortalObject.AddComponent<ScenePortal>();
		newPortal.SetData(portalData);
		BoxCollider2D portalCollider = newPortalObject.AddComponent<BoxCollider2D>();
		portalCollider.isTrigger = true;
	}
}
