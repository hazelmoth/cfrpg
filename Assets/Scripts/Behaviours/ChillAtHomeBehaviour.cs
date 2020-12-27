using SettlementSystem;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class ChillAtHomeBehaviour : IAiBehaviour
{
    private bool running;
    private Actor actor;
    private Coroutine activeCoroutine;
    private IAiBehaviour navSubBehaviour;
    private IAiBehaviour wanderSubBehaviour;
    private SettlementManager settlement;
    private SettlementSystem.House house;

    public ChillAtHomeBehaviour(Actor actor)
    {
        this.actor = actor;
        settlement = GameObject.FindObjectOfType<SettlementManager>();
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

    private void HouseEntranceReached(bool success)
    {
        if (!success)
        {
            // Navigation failed; try restarting the behaviour
            Cancel();
            Execute();
            return;
        }
        ScenePortalActivator.Activate(actor, house.GetComponentInChildren<ScenePortal>());
        wanderSubBehaviour = new WanderBehaviour(actor);
        wanderSubBehaviour.Execute();
    }
}
