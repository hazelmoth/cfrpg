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
			Vector2 spawnPoint = ActorSpawnpointFinder.FindSpawnPoint(SceneObjectManager.WorldSceneId);
			ActorRegistry.RegisterActor(playerData);
			Actor player = ActorSpawner.Spawn(playerData.actorId, spawnPoint, SceneObjectManager.WorldSceneId);
			PlayerController.SetPlayerActor(playerData.actorId);

			player.GetData().Inventory.AttemptAddItem(new ItemStack("seed_bag", 1));


			#region StartWagonPlacement

			// TODO force the wagon placement somehow so we never start without a wagon
			Vector2Int wagonLocation = TilemapInterface
				.WorldPosToScenePos(player.transform.position, player.CurrentScene).ToVector2Int();

			wagonLocation += Vector2Int.right * 10;

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
					wagonInv.AttemptAddItem(new ItemStack(ContentLibrary.Instance.Items.Get("log")));
				}
				for (int i = 0; i < 12; i++)
				{
					wagonInv.AttemptAddItem(new ItemStack(ContentLibrary.Instance.Items.Get("bear_fur")));
				}
			}

			#endregion

			// TODO get this done during world generation
			#region ShackPlacement

			Vector2Int houseLocation = TilemapInterface
				.WorldPosToScenePos(player.transform.position, player.CurrentScene).ToVector2Int();
			string houseScene = player.CurrentScene;

			bool housePlaced = WorldMapManager.AttemptPlaceEntityAtPoint(
				ContentLibrary.Instance.Entities.Get("shack"), houseLocation, houseScene);

			#endregion

		}
	}
}
