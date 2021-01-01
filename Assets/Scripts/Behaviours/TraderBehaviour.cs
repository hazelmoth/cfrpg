using System.Collections;
using System.ComponentModel;
using UnityEngine;

public class TraderBehaviour : IAiBehaviour
{
    private const float StayDuration = 120; // Time, in seconds, that traders stick around
    private const float FollowTargetDist = 3.5f; // How closely to follow the player
    private const float FollowBuffer = 6; // The buffer distance outside the target dist

    private bool running;
    private float startTime;
    private readonly Actor actor;
    private IAiBehaviour followPlayerBehaviour;
    private IAiBehaviour moveToEdgeBehaviour;
    private Coroutine currentCoroutine;

    public bool IsRunning => running;

    public TraderBehaviour(Actor actor)
    {
        this.actor = actor;
    }

    public void Cancel()
    {
        actor.StopCoroutine(currentCoroutine);
        running = false;
    }

    public void Execute()
    {
        startTime = Time.time;
        Actor player = ActorRegistry.Get(PlayerController.PlayerActorId).actorObject;
        followPlayerBehaviour = new FollowBehaviour(actor, player, FollowTargetDist, FollowBuffer);
        followPlayerBehaviour.Execute();
        currentCoroutine = actor.StartCoroutine(WaitToLeaveCoroutine());
        running = true;
    }

    private IEnumerator WaitToLeaveCoroutine ()
    {
        while (Time.time - startTime < StayDuration)
        {
            yield return null;
        }
        followPlayerBehaviour.Cancel();

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
            // Exiting map failed.
            Debug.LogWarning("Trader couldn't find a way out of the map", actor);
            Cancel();
        }
    }
}
