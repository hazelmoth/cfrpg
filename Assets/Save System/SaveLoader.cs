using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoader
{
	public delegate void SaveLoaderCallback();
	public delegate void SaveLoadedEvent();
	public static SaveLoadedEvent OnSaveLoaded;

	public static void LoadSave(WorldSave save, SavedPlayerChar player, SaveLoaderCallback callback)
    {
		IEnumerator coroutine = LoadSaveCoroutine(save, player, callback);
		GlobalCoroutineObject.Instance.StartCoroutine(coroutine);
    }
	static IEnumerator LoadSaveCoroutine(WorldSave save, SavedPlayerChar player, SaveLoaderCallback callback)
	{
		GameDataMaster.WorldName = save.worldName;

		WorldMapManager.LoadMap(save.worldMap.ToNonSerializable());

        foreach (SavedEntity entity in save.entities)
        {
            GameObject entityObject = WorldMapManager.GetEntityObjectAtPoint(entity.location.ToVector2Int(), entity.scene);
            EntityObject entityObjectObject = entityObject.GetComponent<EntityObject>();
            entityObjectObject.SetStateData(entity);
        }
		foreach (SerializableScenePortal portalData in save.scenePortals)
		{
			// Scene portals owned by entities should be saved and loaded with SaveableComponents on their entities, not here
			if (portalData.ownedByEntity)
			{
				continue;
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
				newPortalObject.transform.position = TilemapInterface.ScenePosToWorldPos(portalData.sceneRelativeLocation, portalData.portalScene);
				ScenePortal newPortal = newPortalObject.AddComponent<ScenePortal>();
				newPortal.SetData(portalData);
				BoxCollider2D portalCollider = newPortalObject.AddComponent<BoxCollider2D>();
				portalCollider.isTrigger = true;
			}
		}
		ScenePortalLibrary.BuildLibrary();

		if (!SceneObjectManager.SceneExists(player.scene))
		{
			Debug.LogError("Saved player \"" + player.data.saveId + "\" is in a scene \"" + player.scene + "\" that doesn't currently exist!\nPlacing player in world scene.");
			player.scene = SceneObjectManager.WorldSceneId;
		}
		PlayerSpawner.Spawn(player.data, player.scene, player.location);
		PlayerDucats.SetDucatBalance(player.data.ducatBalance);

		foreach(SavedNpc savedNpc in save.npcs)
		{
			NPCData npc = savedNpc.data.ToNonSerializable();
			NPCDataMaster.AddNPC(npc);

			ActorData data = new ActorData(npc.NpcId, npc.NpcName, npc.Personality, npc.RaceId, npc.HairId, new ActorPhysicalCondition(), new ActorInventory(), new FactionStatus(null));
			ActorRegistry.RegisterActor(data, null);

			NPC spawnedNpc = NPCSpawner.Spawn(npc.NpcId, savedNpc.location, savedNpc.scene, savedNpc.direction);

			spawnedNpc.GetData().Inventory.SetInventory(savedNpc.data.invContents.ToNonSerializable());
		}

		OnSaveLoaded?.Invoke();
		callback?.Invoke();
		yield return null;
	}
}
