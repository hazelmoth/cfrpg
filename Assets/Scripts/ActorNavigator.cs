using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// An ActorNavigator controls its actor as it moves along a given path.
// It does not perform any pathfinding on its own.
// Directly interfaces the ActorMovementController; nothing else should.
public class ActorNavigator : MonoBehaviour
{
	public delegate void ActorNavigationEvent();
	public delegate void ActorNavigationEventFailable(bool didSucceed);
	public delegate void ActorNavigationEventObstacleFailable(bool didSucceed, Vector2Int obstacleWorldPos);
	public event ActorNavigationEvent NavigationCompleted;

	private const float OBSTACLE_WAIT_TIMEOUT = 1f;

	private ActorMovementController movement;
	private Actor actor;
	private Vector2? nextPathTile = null;
	public bool debugPath = false;

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
	public void FollowPath(IList<Vector2> path, string scene, ActorNavigationEventObstacleFailable callback)
	{

		CancelNavigation();
		// convert scene space back to world space
		List<Vector2> convertedPath = new List<Vector2>();
		foreach (Vector2 vector in path)
		{
			Vector2 newVector = TilemapInterface.ScenePosToWorldPos(vector, scene);
			convertedPath.Add(newVector);
		}
		StartCoroutine(FollowPathCoroutine(convertedPath, callback));
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

	private void Walk(Vector2 destination, ActorNavigationEventObstacleFailable callback)
	{
		Vector2 startPos = transform.position;
		Vector2 endPos = destination;
		Vector2 move = endPos - startPos;
		movement.SetWalking(move.normalized);
		StopCoroutine("WalkCoroutine");
		StartCoroutine(WalkCoroutine(transform.position, move, Vector2.Distance(startPos, endPos), callback));
	}


	private IEnumerator FollowPathCoroutine(List<Vector2> worldPath, ActorNavigationEventObstacleFailable callback)
	{
		bool didSucceed = false;
		Vector2Int discoveredObstacle = Vector2Int.zero;

		if (debugPath)
			DebugPath(worldPath);

		for (int i = 0; i < worldPath.Count; i++)
		{
			Vector2 destination = worldPath[i];
			if (i < worldPath.Count - 1)
			{
				nextPathTile = worldPath[i + 1];
			}
			else
			{
				nextPathTile = null;
			}

			// Move the destination to the center of its tile
			Vector2 destCenter = TilemapInterface.GetCenterPositionOfTile(TilemapInterface.FloorToTilePos(destination));

			Vector2 startPos = transform.position;

			float distance = Vector2.Distance(startPos, destCenter);
			bool walkFinished = false;

			Walk(destCenter, (success, obstaclePos) =>
			{
				walkFinished = true;
				didSucceed = success;
				discoveredObstacle = obstaclePos;
			});

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
		NavigationCompleted?.Invoke();
		callback?.Invoke(didSucceed, discoveredObstacle);
	}
	// Makes the actor walk a given distance. Calls back false if an obstacle
	// blocks its path for longer than the obstacle wait timeout.
	private IEnumerator WalkCoroutine(Vector2 startPos, Vector2 velocity, float distance, ActorNavigationEventObstacleFailable callback)
	{

		bool didSucceed = true;
		bool waitingAtObstacle = false;
		float waitStartTime = 0;
		Vector2Int obstacleLocation = Vector2Int.zero;

		while (Vector2.Distance(startPos, transform.position) <= distance)
		{
			// TODO make sure we're always pointing the right way

			if (nextPathTile.HasValue && ObstacleDetectionSystem.CheckForObstacles(actor, nextPathTile.Value))
			{
				movement.SetWalking(Vector2.zero);
				if (!waitingAtObstacle)
				{
					waitingAtObstacle = true;
					waitStartTime = Time.time;
				}
				else
				{
					if (Time.time - waitStartTime > OBSTACLE_WAIT_TIMEOUT)
					{
						didSucceed = false;
						obstacleLocation = nextPathTile.Value.ToVector2Int();
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


	public void DebugPath(List<Vector2> worldPath)
	{

		LineRenderer liner = GetComponent<LineRenderer>();
		if (liner == null)
			liner = gameObject.AddComponent<LineRenderer>();

		Vector3[] linePoints = new Vector3[worldPath.Count];

		for (int i = 0; i < worldPath.Count; i++)
		{
			linePoints[i] = new Vector3(worldPath[i].x + 0.5f, worldPath[i].y + 0.5f, -2f);
		}
		liner.startWidth = 0.1f;
		liner.endWidth = 0.1f;
		liner.positionCount = linePoints.Length;
		liner.startColor = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, 0.6f);
		liner.endColor = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, 0.6f);
		liner.material = (Material)Resources.Load("DebugMaterial");
		liner.SetPositions(linePoints);
		NavigationCompleted += HideDebugPath;
	}
	public void HideDebugPath()
	{
		LineRenderer liner = gameObject.GetComponent<LineRenderer>();
		if (liner != null)
		{
			GameObject.Destroy(liner);
		}
	}
}
