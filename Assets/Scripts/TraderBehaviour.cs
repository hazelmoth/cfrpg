using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraderBehaviour : IAiBehaviour
{
    // time, in seconds, that traders stick around
    private const float StayDuration = 120;

    private bool running;
    private float startTime;
    private readonly Actor actor;
    private IAiBehaviour wanderBehaviour;
    private IAiBehaviour moveToEdgeBehaviour;
    private Coroutine currentCoroutine;

    bool IAiBehaviour.IsRunning => running;

    public TraderBehaviour(Actor actor)
    {
        this.actor = actor;
    }

    void IAiBehaviour.Cancel()
    {
        actor.StopCoroutine(currentCoroutine);
        running = false;
    }

    void IAiBehaviour.Execute()
    {
        startTime = Time.time;
        wanderBehaviour = new WanderBehaviour(actor);
        wanderBehaviour.Execute();
        currentCoroutine = actor.StartCoroutine(WaitToLeaveCoroutine());
        running = true;
    }

    private IEnumerator WaitToLeaveCoroutine ()
    {
        while (Time.time - startTime < StayDuration)
        {
            yield return null;
        }
        wanderBehaviour.Cancel();

        // Deliver a notification that the actor is leaving
        NotificationManager.Notify(actor.GetData().ActorName + " is leaving the area.");

        Vector2Int exitPos = WorldMapManager.FindWalkableEdgeTile(Direction.Up);
        TileLocation exitTile = new TileLocation(exitPos, SceneObjectManager.WorldSceneId);
        bool exitSucceeded = false;
        moveToEdgeBehaviour = new NavigateBehaviour(actor, exitTile, (bool success) => exitSucceeded = success);
        moveToEdgeBehaviour.Execute();
        while (moveToEdgeBehaviour.IsRunning)
        {
            yield return null;
        }
        if (exitSucceeded)
        {
            // We reached the map edge successfully. Time to destroy this actor object.
            running = false;
            Object.Destroy(actor.gameObject);
        } 
        else
        {
            // Exiting map failed. Try it again.
            running = true;
            currentCoroutine = actor.StartCoroutine(WaitToLeaveCoroutine()); //TODO: make this not stack overflow when exiting is impossible
        }
    }
}
