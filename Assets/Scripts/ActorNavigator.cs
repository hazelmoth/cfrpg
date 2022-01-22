using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Popcron.Console;
using UnityEngine;

// An ActorNavigator controls its actor as it moves along a given path.
// It does not perform any pathfinding on its own.
// Directly interfaces the ActorMovementController.
public class ActorNavigator : MonoBehaviour
{
	public delegate void ActorNavigationEvent();
	public delegate void ActorNavigationEventObstacleFailable(bool didSucceed, Vector2Int obstacleWorldPos);
	public event ActorNavigationEvent OnNavigationCompleted;

	private const float ObstacleWaitTimeout = 1f;

	private ActorMovementController movement;
	private Actor actor;
	private Vector2? nextPathStep = null;

	[Command("debugpaths")][UsedImplicitly]
	public static bool debugPaths = false;

	// Use this for initialization
	private void Awake()
	{
		movement = GetComponent<ActorMovementController>();
		if (movement == null)
		{
			Debug.LogError("Actor is missing a movement controller!");
		}
		actor = GetComponent<Actor>();
	}

	// Takes a path in scene space. Assumes given path is not null.
	public void FollowPath(
		IList<Vector2> path,
		string scene,
		ActorNavigationEventObstacleFailable callback,
		bool adjustToTileCenter = true,
		Actor ignored = null)
	{
		CancelNavigation();

		// If the path length is zero, this is really easy
		if (path.Count == 0)
		{
			Debug.Log("Path had zero length.");
			callback.Invoke(true, Vector2Int.zero);
			return;
		}

		// Convert scene space back to world space
		List<Vector2> convertedPath = new List<Vector2>();
		foreach (Vector2 vector in path)
		{
			Vector2 newVector = TilemapInterface.ScenePosToWorldPos(vector, scene);
			convertedPath.Add(newVector);
		}
		StartCoroutine(FollowPathCoroutine(convertedPath, callback, adjustToTileCenter, ignored));
	}
	public void CancelNavigation()
	{
		StopAllCoroutines();
		movement.SetWalking(Vector2.zero);
	}
	public void ForceDirection(Direction dir)
	{
		movement.ForceDirection(dir);
	}

	private void Walk(Vector2 destination, ActorNavigationEventObstacleFailable callback, Actor ignored = null)
	{
		Vector2 startPos = transform.position;
		Vector2 endPos = destination;
		Vector2 move = endPos - startPos;
		movement.SetWalking(move.normalized);
		StopCoroutine("WalkCoroutine");
		StartCoroutine(WalkCoroutine(startPos, move.normalized, Vector2.Distance(startPos, endPos), callback, ignored));
	}


	private IEnumerator FollowPathCoroutine(
		List<Vector2> worldPath,
		ActorNavigationEventObstacleFailable callback,
		bool adjustToTileCenter,
		Actor ignored = null)
	{
		bool didSucceed = false;
		Vector2Int discoveredObstacle = Vector2Int.zero;

		if (debugPaths)
			DebugPath(worldPath);

		for (int i = 0; i < worldPath.Count; i++)
		{
			Vector2 destination = worldPath[i];
			if (i < worldPath.Count - 1)
			{
				nextPathStep = worldPath[i];
			}
			else
			{
				nextPathStep = null;
			}

			bool walkFinished = false;

			Walk(destination, (success, obstaclePos) =>
			{
				walkFinished = true;
				didSucceed = success;
				discoveredObstacle = obstaclePos;
			}, ignored);

			while (!walkFinished)
			{
				yield return null;
			}
			movement.SetWalking(Vector2.zero);

			// Don't continue following the path if we've been blocked by an obstacle
			if (!didSucceed)
			{
				break;
			}
		}
		OnNavigationCompleted?.Invoke();
		callback?.Invoke(didSucceed, discoveredObstacle);
	}

	/// Makes the actor walk a given distance. Calls back false if an obstacle
	/// blocks its path for longer than the obstacle wait timeout. If an Actor
	/// is given, doesn't consider that Actor an obstacle (useful for melee combat).
	private IEnumerator WalkCoroutine(Vector2 startPos, Vector2 velocity, float distance, ActorNavigationEventObstacleFailable callback, Actor ignored = null)
	{
		bool didSucceed = true;
		bool waitingAtObstacle = false;
		float waitStartTime = 0;
		Vector2Int obstacleLocation = Vector2Int.zero;

		while (Vector2.Distance(startPos, transform.position) <= distance)
		{
			// TODO make sure we're always pointing the right way

			if (nextPathStep.HasValue && ObstacleDetectionSystem.CheckForObstacles(actor, nextPathStep.Value, ignored))
			{
				movement.SetWalking(Vector2.zero);
				if (!waitingAtObstacle)
				{
					waitingAtObstacle = true;
					waitStartTime = Time.time;
				}
				else
				{
					if (Time.time - waitStartTime > ObstacleWaitTimeout)
					{
						didSucceed = false;
						obstacleLocation = TilemapInterface.FloorToTilePos(nextPathStep.Value);
						break;
					}
				}
			}
			else
			{
				movement.SetWalking(velocity);
				waitingAtObstacle = false;
			}
			yield return null;
		}
		callback?.Invoke(didSucceed, obstacleLocation);
		movement.SetWalking(Vector2.zero);
	}


	private void DebugPath(List<Vector2> worldPath)
	{

		LineRenderer liner = GetComponent<LineRenderer>();
		if (liner == null)
			liner = gameObject.AddComponent<LineRenderer>();

		Vector3[] linePoints = new Vector3[worldPath.Count];

		for (int i = 0; i < worldPath.Count; i++)
		{
			linePoints[i] = (Vector3)worldPath[i] + Vector3.forward * -2f;
		}
		liner.startWidth = 0.1f;
		liner.endWidth = 0.1f;
		liner.positionCount = linePoints.Length;
		liner.startColor = new Color(Random.value, Random.value, Random.value, 0.6f);
		liner.endColor = new Color(Random.value, Random.value, Random.value, 0.6f);
		liner.material = (Material)Resources.Load("DebugMaterial");
		liner.SetPositions(linePoints);
		OnNavigationCompleted += HideDebugPath;
	}

	private void HideDebugPath()
	{
		LineRenderer liner = gameObject.GetComponent<LineRenderer>();
		if (liner != null)
		{
			Destroy(liner);
		}
	}
}
