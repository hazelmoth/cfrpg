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
				// Fill starting wagon with supplies
				for (int i = 0; i < 24; i++)
				{
					wagonInv.AttemptAddItem(new ItemStack(ContentLibrary.Instance.Items.Get("wood")));
				}
				for (int i = 0; i < 12; i++)
				{
					wagonInv.AttemptAddItem(new ItemStack(ContentLibrary.Instance.Items.Get("bear_fur")));
				}
			}


			Vector2 spawnPoint = ActorSpawnpointFinder.FindSpawnPointNearCoords(SceneObjectManager.WorldSceneId, GameDataMaster.WorldSize / 2);
			ActorRegistry.RegisterActor(playerData);
			Actor player = ActorSpawner.Spawn(playerData.actorId, spawnPoint, SceneObjectManager.WorldSceneId);
			PlayerController.SetPlayerActor(playerData.actorId);

			player.GetData().Inventory.AttemptAddItem(new ItemStack("wheat_seeds", 1)); // Give the player some seeds to start off.
		}
	}
}
