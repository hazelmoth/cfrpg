using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigateToSceneBehaviour : IAiBehaviour
{
    private Actor actor;
    private Action<bool> callback;
    private Coroutine activeCoroutine;
    private IAiBehaviour navSubBehaviour;
    private string targetScene;
    private ScenePortal portal;

    public NavigateToSceneBehaviour(Actor actor, string targetScene, Action<bool> callback)
    {
        this.targetScene = targetScene;
        this.callback = callback;
        this.actor = actor;
    }

    public bool IsRunning { get; private set; }

    public void Cancel()
    {
        if (!IsRunning) return;
        navSubBehaviour?.Cancel();
        IsRunning = false;
        callback?.Invoke(false);
    }

    public void Execute()
    {
        IsRunning = true;
        List<ScenePortal> availablePortals = ScenePortalLibrary.GetPortalsBetweenScenes(actor.CurrentScene, targetScene);
        if (availablePortals.Count == 0)
        {
            Debug.LogWarning("Couldn't find scene portals connecting scenes: " + actor.CurrentScene + " and " + targetScene, actor);
            Cancel();
        }
        portal = availablePortals[0];

        // Find a valid tile next to the scene portal
        Vector2 targetLocation = Pathfinder.GetValidAdjacentTiles(
            portal.PortalScene,
            TilemapInterface.WorldPosToScenePos(portal.transform.position,
            portal.PortalScene),
            null)[0];

        TileLocation targetTile = new TileLocation(targetLocation.ToVector2Int(), portal.PortalScene);
        navSubBehaviour = new NavigateBehaviour(actor, targetTile, ScenePortalReached);
        navSubBehaviour.Execute();
    }

    private void ScenePortalReached(bool success)
    {
        if (!success)
        {
            Cancel();
            return;
        }
        ScenePortalActivator.Activate(actor, portal);
        callback.Invoke(true);
    }
}
