using System.Collections;
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
		GameDataMaster.WorldName = save.worldName;
		GameDataMaster.WorldSize = save.worldSize.ToVector2Int();

		RegionMapManager.LoadMap(save.worldMap.ToNonSerializable());

		TimeKeeper.SetCurrentTick(save.time);

		History history = GameObject.FindObjectOfType<History>();
		history.LoadEventLog(save.eventLog);

        foreach (SavedEntity entity in save.entities)
        {
            GameObject entityObject = RegionMapManager.GetEntityObjectAtPoint(entity.location.ToVector2Int(), entity.scene);
            EntityObject entityObjectObject = entityObject.GetComponent<EntityObject>();
            entityObjectObject.SetStateData(entity);
        }
		foreach (SerializableScenePortal portalData in save.scenePortals)
		{
			// Scene portals owned by entities should be saved and loaded with SaveableComponents on their entities, not here
			if (portalData.ownedByEntity)
			{
			}
			else
			{
				GameObject newPortalObject = new GameObject("Scene portal");
				if (portalData.portalScene == null)
				{
					Debug.LogError("Saved scene portal has no data for what scene it's in! Not loading this portal.");
					continue;
				}
				else if (!SceneObjectManager.SceneExists(portalData.portalScene))
				{
					Debug.LogError("Saved scene portal belongs to a scene \"" + portalData.portalScene + "\" that doesn't currently exist! Not loading this portal.");
					continue;
				}
				newPortalObject.transform.SetParent(SceneObjectManager.GetSceneObjectFromId(portalData.portalScene).transform);
				newPortalObject.transform.position = TilemapInterface.ScenePosToWorldPos(portalData.sceneRelativeLocation.ToVector2(), portalData.portalScene);
				ScenePortal newPortal = newPortalObject.AddComponent<ScenePortal>();
				newPortal.SetData(portalData);
				BoxCollider2D portalCollider = newPortalObject.AddComponent<BoxCollider2D>();
				portalCollider.isTrigger = true;
			}
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

		foreach(SavedActor savedActor in save.actors)
		{
			ActorData data = savedActor.data.ToNonSerializable();
			ActorRegistry.RegisterActor(data);

			Actor spawnedActor = ActorSpawner.Spawn(data.actorId, savedActor.location.ToVector2(), savedActor.scene, savedActor.direction);

			ActorRegistry.RegisterActorGameObject(spawnedActor);
			if (save.playerActorId == data.actorId)
			{
				PlayerController.SetPlayerActor(data.actorId);
			}
		}

		OnSaveLoaded?.Invoke();
		callback?.Invoke();
		yield return null;
	}
}
