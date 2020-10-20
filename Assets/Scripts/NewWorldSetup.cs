using UnityEngine;

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
			InteractableContainer wagonInv = GameObject.FindObjectOfType<InteractableContainer>();
			if (wagonInv != null)
			{
				wagonInv.AttemptAddItems("wood", 48);
				wagonInv.AttemptAddItems("bear_fur", 24);
				wagonInv.AttemptAddItems("wheat_seeds", 4);
				wagonInv.AttemptAddItems("hoe", 1);
				wagonInv.AttemptAddItems("watering_can", 1);
			}

			Vector2 spawnPoint = ActorSpawnpointFinder.FindSpawnPointNearCoords(SceneObjectManager.WorldSceneId, (GameDataMaster.WorldSize / 2) + Vector2.down * 5);
			ActorRegistry.RegisterActor(playerData);
			Actor player = ActorSpawner.Spawn(playerData.actorId, spawnPoint, SceneObjectManager.WorldSceneId);
			PlayerController.SetPlayerActor(playerData.actorId);
		}
	}
}
