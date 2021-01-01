using SettlementSystem;
using UnityEngine;

public class ChillAtHomeBehaviour : IAiBehaviour
{
    private const int maxNavFailures = 1; // After this many nav failures, the behaviour will self-cancel.
    private bool running;
    private Actor actor;
    private Coroutine activeCoroutine;
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

    public bool IsRunning => running;

    public void Cancel()
    {
        if (navSubBehaviour != null && navSubBehaviour.IsRunning) navSubBehaviour.Cancel();
        wanderSubBehaviour?.Cancel();
        running = false;
    }

    public void Execute()
    {
        running = true;
        house = settlement.GetHouse(actor.ActorId);
        ScenePortal portal = house.GetComponentInChildren<ScenePortal>();

        Vector2 targetLocation = Pathfinder.GetValidAdjacentTiles(
            portal.PortalScene,
            TilemapInterface.WorldPosToScenePos(portal.transform.position,
            portal.PortalScene),
            null)[0];

        TileLocation targetTile = new TileLocation(targetLocation.ToVector2Int(), portal.PortalScene);
        navSubBehaviour = new NavigateBehaviour(actor, targetTile, HouseEntranceReached);
        navSubBehaviour.Execute();
    }

    private void HouseEntranceReached(bool success) {
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
