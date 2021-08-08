using ContinentMaps;
using UnityEngine;

/// A class providing static methods for triggering the sequence of events after the
/// player's death.
public static class PlayerDeathSequence
{
    public static void HandleDeath(string playerId)
    {
        Actor player = ActorRegistry.Get(playerId).actorObject;
        
        ScreenFadeAnimator.FadeOut(1);
        GlobalCoroutineObject.InvokeAfter(1, true, () =>
        {
            PauseManager.Pause();
            
            // Drop the player's items
            foreach (ItemStack item in player.GetData().Inventory.GetAllItems())
            {
                DroppedItemSpawner.SpawnItem(item, player.Location.Vector2, player.Location.scene, true);
            }
            player.GetData().Inventory.Clear();
            
            // Locate the home region
            Vector2Int homeRegion = Vector2Int.zero;
            for (int x = 0; x < ContinentManager.LoadedMap.dimensions.x; x++)
            for (int y = 0; y < ContinentManager.LoadedMap.dimensions.y; y++)
            {
                if (!ContinentManager.LoadedMap.regionInfo[x, y].playerHome) continue;
                
                homeRegion = new Vector2Int(x, y);
                break;
            }
            
            RegionTravel.TravelTo(player, homeRegion, false, isSuccessful =>
            {
                if (!PauseManager.Paused) PauseManager.Pause();
                if (!isSuccessful)
                {
                    Debug.LogError("Region travel failed when respawning player!");
                }
                player = ActorRegistry.Get(playerId).actorObject;
                ActorData playerData = player.GetData();
                
                // Wait a bit so the new region is set up, and for dramatic purposes
                GlobalCoroutineObject.InvokeAfter(1, true, () =>
                {
                    // Remove the player and then respawn in a suitable spawn point
                    Object.Destroy(player.gameObject);
                    playerData.Health.ResetHealth();
                    ActorSpawner.Spawn(
                        playerId,
                        ActorSpawnpointFinder.FindSpawnPoint(
                            RegionMapManager.GetRegionMap(),
                            SceneObjectManager.WorldSceneId),
                        SceneObjectManager.WorldSceneId);

                    PauseManager.Unpause();
                    ScreenFadeAnimator.FadeIn(1);
                });
            });
        });
    }
}