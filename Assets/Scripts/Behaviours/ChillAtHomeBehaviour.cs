using SettlementSystem;
using UnityEngine;

namespace Behaviours
{
    public class ChillAtHomeBehaviour : IAiBehaviour
    {
        private const int maxNavFailures = 1; // After this many nav failures, the behaviour will self-cancel.
        private Actor actor;
        private IAiBehaviour navSubBehaviour;
        private IAiBehaviour wanderSubBehaviour;
        private SettlementManager settlement;
        private SettlementSystem.House house;
        private int navFailures;

        public ChillAtHomeBehaviour(Actor actor)
        {
            this.actor = actor;
            settlement = GameObject.FindObjectOfType<SettlementManager>();
            navFailures = 0;
        }

        public bool IsRunning { get; private set; }

        public void Cancel()
        {
            if (!IsRunning) return;

            IsRunning = false;
            navSubBehaviour?.Cancel();
            wanderSubBehaviour?.Cancel();
        }

        public void Execute()
        {
            navSubBehaviour?.Cancel();
            wanderSubBehaviour?.Cancel();

            IsRunning = true;
            house = settlement.GetHouse(actor.ActorId);
            ScenePortal portal = house.GetComponentInChildren<ScenePortal>();

            // Pick any valid location next to the target portal
            Vector2 targetLocation = Pathfinder.GetValidAdjacentTiles(
                portal.PortalScene,
                TilemapInterface.WorldPosToScenePos(portal.transform.position,
                    portal.PortalScene),
                null).PickRandom();

            TileLocation targetTile = new TileLocation(targetLocation.ToVector2Int(), portal.PortalScene);
            navSubBehaviour = new NavigateBehaviour(actor, targetTile, HouseEntranceReached);
            navSubBehaviour.Execute();
        }

        private void HouseEntranceReached(bool success) 
        {
            if (!IsRunning) return;

            if (!success)
            {
                navFailures++;
                Cancel();
                if (navFailures < maxNavFailures)
                {
                    // Navigation failed; try restarting the behaviour
                    Execute();
                }
                return;
            }
            ScenePortalActivator.Activate(actor, house.GetComponentInChildren<ScenePortal>());
            wanderSubBehaviour = new WanderBehaviour(actor);
            wanderSubBehaviour.Execute();
        }
    }
}
