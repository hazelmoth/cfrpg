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

		SceneLoader.LoadScenes (AfterScenesLoaded);
	}
	void AfterScenesLoaded () {

		// Load any mod assets

		// TEST
		WorldMapManager.LoadMap(WorldMapGenerator.Generate(50, 50));
		WorldMapManager.LoadMapsIntoScenes();
		Debug.Log(WorldMapManager.AttemptPlaceEntityAtPoint(EntityLibrary.GetEntityFromID("tent_green"), new Vector2Int(0, 0), "World"));

        // TEST
        for (int n = 0; n < 20; n++)
        {
            NPCData newNpc = NPCGenerator.Generate();
            NPCDataMaster.AddNPC(newNpc);
            NPC npc = NPCSpawner.Spawn(newNpc.NpcId, new Vector2(110, 5), "World");
            npc.GetComponent<NPCTaskExecutor>().Wander();
        }

        // TEST obviously temporary
        PlayerDucats.SetDucatBalance (666);


		NotificationManager.Notify ("We're go.");
	}
}

