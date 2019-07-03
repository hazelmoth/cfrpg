using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Manages the whole launch sequence (scene loading, asset loading, etc)
public class GameInitializer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
	{
		EntityLibrary.LoadLibrary ();
		GroundMaterialLibrary.LoadLibrary ();
		SceneObjectManager.Initialize (); 
		InitialSceneLoader.LoadScenes (AfterScenesLoaded);
		Random.InitState((int)System.DateTime.Now.Ticks);


	}
	void AfterScenesLoaded () {

		// Load any mod assets

		// TEST
		WorldMapManager.LoadMap(WorldMapGenerator.Generate(200, 200));
		WorldMapManager.LoadMapsIntoScenes();

        // TEST
        for (int n = 0; n < 8; n++)
        {
            NPCData newNpc = NPCGenerator.Generate();
            NPCDataMaster.AddNPC(newNpc);
			NPC npc = NPCSpawner.Spawn(newNpc.NpcId, new Vector2(100, 100), SceneObjectManager.WorldSceneId);
            npc.GetComponent<NPCActivityExecutor>().Execute_Wander();
        }

        // TEST obviously temporary
        PlayerDucats.SetDucatBalance (666);

		//TEST
		DroppedItemSpawner.SpawnItem("log", new Vector2(10, 10), SceneObjectManager.WorldSceneId, true);

		NotificationManager.Notify ("We're go.");
	}
}

