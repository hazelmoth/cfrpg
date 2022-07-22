using ActorComponents;
using ContinentMaps;
using UnityEngine;

namespace IntroSequences
{
    /// An intro sequence that just spawns the player at a random position.
    [CreateAssetMenu(fileName = "IntroSequence", menuName = "IntroSequence/RandomSpawn")]
    public class RandomSpawnIntroSequence : IntroSequence
    {
        [SerializeField] private CompoundWeightedTable inventoryTable;
        public override void Run(GameObject cameraRigPrefab, string playerActorId)
        {
            Vector2 spawnPoint = ActorSpawnpointFinder.FindSpawnPoint(
                ContinentManager.CurrentRegion.data,
                SceneObjectManager.WorldSceneId);

            Actor player = ActorSpawner.Spawn(playerActorId, spawnPoint, SceneObjectManager.WorldSceneId);
            PlayerController.SetPlayerActor(playerActorId);
            ActorInventory playerInv = player.GetData().Get<ActorInventory>();

            // Set the player's starting inventory.
            inventoryTable?.Pick().ForEach(id => playerInv?.AttemptAdd(id, 1));
        }
    }
}
