using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewGameSetup : MonoBehaviour
{
	public static void PerformSetup()
	{
		for (int i = 0; i < 8; i++)
		{
			Vector2 spawnPoint = ActorSpawnpointFinder.FindSpawnPoint(SceneObjectManager.WorldSceneId);
			NPCData data = NPCGenerator.Generate();
			NPCDataMaster.AddNPC(data);
			string id = data.NpcId;
			NPC npc = NPCSpawner.Spawn(id, spawnPoint, SceneObjectManager.WorldSceneId);
		}
	}
}
