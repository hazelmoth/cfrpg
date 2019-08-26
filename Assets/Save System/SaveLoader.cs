using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoader : MonoBehaviour
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

		PlayerSpawner.Spawn(player.data, player.scene, player.location);
		PlayerDucats.SetDucatBalance(player.data.ducatBalance);

		foreach(SavedNpc savedNpc in save.npcs)
		{
			NPCData npc = savedNpc.data.ToNonSerializable();
			NPCDataMaster.AddNPC(npc);
			NPC spawnedNpc = NPCSpawner.Spawn(npc.NpcId, savedNpc.location, savedNpc.scene, savedNpc.direction);

			spawnedNpc.Inventory.SetInventory(savedNpc.data.invContents.ToNonSerializable());
		}

		OnSaveLoaded?.Invoke();
		callback?.Invoke();
		yield return null;
	}
}
