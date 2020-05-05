using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Contains methods for setting up a new world after it is loaded for the first time
public static class NewGameSetup
{
	public static void PerformSetup()
	{
		// Spawn the newly created player
		ActorData playerData = GameDataMaster.NewlyCreatedPlayer;
		if (playerData == null)
		{
			Debug.LogError("No player data found for new world!");
		}
		else
		{
			Vector2 spawnPoint = ActorSpawnpointFinder.FindSpawnPoint(SceneObjectManager.WorldSceneId);
			ActorRegistry.RegisterActor(playerData);
			ActorSpawner.Spawn(playerData.actorId, spawnPoint, SceneObjectManager.WorldSceneId);
			PlayerController.SetPlayerActor(playerData.actorId);
		}

		// Spawn 8 Actors in random locations
		for (int i = 0; i < 8; i++)
		{
			Vector2 spawnPoint = ActorSpawnpointFinder.FindSpawnPoint(SceneObjectManager.WorldSceneId);
			ActorData data = ActorGenerator.Generate();
			string id = data.actorId;
			ActorRegistry.RegisterActor(data);
			Actor actor = ActorSpawner.Spawn(id, spawnPoint, SceneObjectManager.WorldSceneId);

		}
		// Spawn 8 bears
		for (int i = 0; i < 8; i++)
		{
			Vector2 spawnPoint = ActorSpawnpointFinder.FindSpawnPoint(SceneObjectManager.WorldSceneId);
			ActorData data = ActorGenerator.GenerateAnimal("bear");
			ActorRegistry.RegisterActor(data);
			string id = data.actorId;
			ActorSpawner.Spawn(id, spawnPoint, SceneObjectManager.WorldSceneId);
		}
	}
}
