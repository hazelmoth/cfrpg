using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Contains methods for setting up a new world after it is loaded for the first time
public static class NewGameSetup
{
	public static void PerformSetup()
	{
		// Spawn 8 NPCs in random locations
		for (int i = 0; i < 8; i++)
		{
			Vector2 spawnPoint = ActorSpawnpointFinder.FindSpawnPoint(SceneObjectManager.WorldSceneId);
			NPCData data = NPCGenerator.Generate();
			NPCDataMaster.AddNPC(data);
			string id = data.NpcId;
			NPC npc = NPCSpawner.Spawn(id, spawnPoint, SceneObjectManager.WorldSceneId);

			// TEST 50% chance to make this npc a follower of the player
			if (Random.value > 0.5f)
			{
				npc.SettlementData.LeaderId = Player.instance.ActorId;
				NotificationManager.Notify(npc.name + " has joined your crew!");
			}
		}
	}
}
