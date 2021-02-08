using UnityEngine;

// Contains methods for setting up a new world after it is loaded for the first time
public static class NewWorldSetup
{
	public static void PerformSetup()
	{
		// Spawn the newly created player
		ActorData playerData = SaveInfo.NewlyCreatedPlayer;
		if (playerData == null)
		{
			Debug.LogError("No player data found for new world!");
		}
		else
		{
			// Find the starting wagon and fill it with supplies.
			InteractableContainer wagonInv = GameObject.FindObjectOfType<InteractableContainer>();
			FillStartingWagon(wagonInv);

			Vector2 spawnPoint = ActorSpawnpointFinder.FindSpawnPointNearCoords(SceneObjectManager.WorldSceneId, (SaveInfo.RegionSize / 2) + Vector2.down * 5);
			ActorRegistry.RegisterActor(playerData);
			Actor player = ActorSpawner.Spawn(playerData.actorId, spawnPoint, SceneObjectManager.WorldSceneId);
			PlayerController.SetPlayerActor(playerData.actorId);
		}
	}

	private static void FillStartingWagon(InteractableContainer wagonInv)
	{
		if (wagonInv == null) return;
		wagonInv.AttemptAddItems("wood", 48);
		wagonInv.AttemptAddItems("bear_fur", 24);
		wagonInv.AttemptAddItems("wheat_seeds", 4);
		wagonInv.AttemptAddItems("hoe", 1);
		wagonInv.AttemptAddItems("watering_can", 1);
	}
}
