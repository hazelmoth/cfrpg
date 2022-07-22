using System.Linq;
using ActorComponents;
using ContinentMaps;
using UnityEngine;

/// A class providing static methods for triggering the sequence of events after the
/// player's death.
public static class PlayerDeathSequence
{
    public static void HandleDeath(string playerId)
    {
        Actor player = ActorRegistry.Get(playerId).actorObject;
        ActorInventory inventory = player.GetData().Get<ActorInventory>();
        ActorHealth health = player.GetData().Get<ActorHealth>();
        
        ScreenFadeAnimator.FadeOut(1);
        GlobalCoroutineObject.InvokeAfter(1, true, () =>
        {
            PauseManager.Pause();

            if (inventory != null)
            {
                // Drop the player's items
                foreach (ItemStack item in inventory.GetAllItems())
                {
                    DroppedItemSpawner.SpawnItem(item, player.Location.Vector2, player.Location.scene, true);
                }

                inventory.Clear();
            }

            // Locate the home region
            string homeRegion = ContinentManager.LoadedMap.regions.Where(region => region.info.playerHome)
                .Select(region => region.Id).FirstOrDefault();

            if (homeRegion == null)
            {
                Debug.LogError("Failed to find a home region to respawn the player at! " +
                               "Choosing a region arbitrarily.");
                homeRegion = ContinentManager.LoadedMap.regions.First().Id;
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
                    health?.Reset();
                    Object.Destroy(player.gameObject);
                    ActorSpawner.Spawn(
                        playerId,
                        ActorSpawnpointFinder.FindSpawnPoint(
                            RegionMapManager.ExportRegionMap(),
                            SceneObjectManager.WorldSceneId),
                        SceneObjectManager.WorldSceneId);

                    PauseManager.Unpause();
                    ScreenFadeAnimator.FadeIn(1);
                });
            });
        });
    }
}
