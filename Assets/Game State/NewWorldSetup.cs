using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Contains methods for setting up a new world after it is loaded for the first time
public static class NewWorldSetup
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
			Actor player = ActorSpawner.Spawn(playerData.actorId, spawnPoint, SceneObjectManager.WorldSceneId);
			PlayerController.SetPlayerActor(playerData.actorId);


			#region StartWagonPlacement

			// TODO force the wagon placement somehow so we never start without a wagon
			Vector2Int wagonLocation = TilemapInterface
				.WorldPosToScenePos(player.transform.position, player.CurrentScene).ToVector2Int();
			string wagonScene = player.CurrentScene;

			bool wagonPlaced = WorldMapManager.AttemptPlaceEntityAtPoint(
				ContentLibrary.Instance.Entities.Get("wagon"), wagonLocation, wagonScene);

			GameObject wagon = WorldMapManager.GetEntityObjectAtPoint(wagonLocation, wagonScene);
			if (wagonPlaced)
			{
				InteractableContainer wagonInv = wagon.GetComponent<InteractableContainer>();

				// Fill starting wagon with supplies
				for (int i = 0; i < 24; i++)
				{
					wagonInv.AttemptAddItem(ContentLibrary.Instance.Items.Get("log"));
				}
				for (int i = 0; i < 12; i++)
				{
					wagonInv.AttemptAddItem(ContentLibrary.Instance.Items.Get("bear_fur"));
				}
			}

			#endregion

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
