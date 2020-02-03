using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Manages the whole launch sequence (scene loading, asset loading, etc)
public class GameInitializer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
	{
		SceneObjectManager.Initialize ();
		Random.InitState((int)System.DateTime.Now.Ticks);

		if (GameDataMaster.SaveToLoad == null)
		{
			Debug.LogError("Scene started with no save loaded!");
		}
		else
		{
			SaveLoader.LoadSave(GameDataMaster.SaveToLoad, GameDataMaster.PlayerToLoad, AfterSaveLoaded);
		}
		
	}
	void AfterSaveLoaded () {


		// TEST
		for (int n = 0; n < 8; n++)
		{
			NPCData newNpc = NPCGenerator.Generate();
			NPCDataMaster.AddNPC(newNpc);
			Vector2 spawn = ActorSpawnpointFinder.FindSpawnPointNearCoords(SceneObjectManager.WorldSceneId, new Vector2(100, 100));
			NPC npc = NPCSpawner.Spawn(newNpc.NpcId, spawn, SceneObjectManager.WorldSceneId);
			npc.GetComponent<NPCActivityExecutor>().Execute_Wander();
		}

		// TEST obviously temporary
		PlayerDucats.SetDucatBalance (666);


		NotificationManager.Notify ("We're go.");

        //TEST
        GameSaver.SaveGame(GameDataMaster.SaveFileId);
	}
}

