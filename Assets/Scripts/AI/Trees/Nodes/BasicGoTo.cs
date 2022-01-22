using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using UnityEngine;

namespace AI.Trees.Nodes
{
	public class BasicGoTo : Node
	{
		private const int MaxRetries = 10; // How many times we'll recalculate a blocked
		// route before returning a failure.

		private readonly Actor agent;
		private readonly Location target;
		private readonly float margin;
		private readonly Actor ignoreCollisionWithActor;
		private ActorNavigator nav;
		private IDictionary<string, ISet<Vector2Int>> blockedTiles;

		private IList<PathSegment> paths; // A path through each scene to the destination
		private int currentSegment; // Which segment of the path will be followed next
		private bool waitingForNavigation;
		private bool lastNavFailed;
		private int failedAttempts;

		public BasicGoTo(Actor agent, Location target, float margin, Actor ignoreCollisionWithActor = null)
		{
			this.agent = agent;
			this.target = target;
			this.margin = margin;
			this.ignoreCollisionWithActor = ignoreCollisionWithActor;
		}

		protected override void Init()
		{
			nav = agent.GetComponent<ActorNavigator>();
			blockedTiles = new Dictionary<string, ISet<Vector2Int>>();
			paths = FindPaths();
		}

		protected override void OnCancel()
		{
			if (waitingForNavigation)
			{
				nav.CancelNavigation();
			}
		}

		protected override Status OnUpdate()
		{
			if (target.scene == agent.Location.scene
				&& Vector2.Distance(target.Vector2, agent.Location.Vector2) < margin)
			{
				// We're already there! instant success.
				nav.CancelNavigation();
				return Status.Success;
			}

			if (lastNavFailed)
			{
				// Path was blocked. Use a retry.
				if (failedAttempts < MaxRetries)
				{
					failedAttempts++;
					paths = FindPaths();
					currentSegment = 0;
				}
				else
				{
					// Out of retries; couldn't find a path.
					return Status.Failure;
				}
			}

			if (paths == null)
			{
				// No path found to the target.
				nav.CancelNavigation();
				return Status.Failure;
			}

			if (currentSegment == paths.Count)
			{
				// We've completed all segments, but haven't met the success condition.
				// Consider this a failure. This shouldn't happen.
				nav.CancelNavigation();
				Debug.LogWarning(
					"Navigation result was unsatisfactory. Distance: "
					+ $"{Vector2.Distance(target.Vector2, agent.Location.Vector2)}; Margin: {margin}",
					agent);
				return Status.Failure;
			}

			if (waitingForNavigation) return Status.Running;

			// Set the navigator on the next path.
			nav.FollowPath(
				paths[currentSegment].path,
				agent.CurrentScene,
				NavFinished,
				ignored: ignoreCollisionWithActor);
			waitingForNavigation = true;

			return Status.Running;
		}

		private void NavFinished(bool success, Vector2Int obstaclePos)
		{
			if (!success)
			{
				if (!blockedTiles.ContainsKey(agent.CurrentScene))
					blockedTiles.Add(agent.CurrentScene, new HashSet<Vector2Int>());
				blockedTiles[agent.CurrentScene].Add(obstaclePos);
			}
			else if (currentSegment != paths.Count - 1)
			{
				// We navigated successfully, but this isn't the last segment.
				Debug.Assert(paths[currentSegment].portal != null, "Missing portal to next segment.");
				// Assert that we're actually somewhat near the portal.
				Debug.Assert(
					Vector2.Distance(paths[currentSegment].portal.transform.position, agent.transform.position) < 3f,
					"Navigation to next segment was successful, but we're not within margin of the portal.");

				// We're at the portal. Go to the next segment.
				ScenePortalActivator.Activate(agent, paths[currentSegment].portal);
			}

			lastNavFailed = !success;
			waitingForNavigation = false;
			currentSegment++;
		}

		private struct PathSegment
		{
			public IList<Vector2> path;
			public ScenePortal portal; // Null if this is the last segment
		}

		/// Finds paths through the scenes leading to the destination.
		private IList<PathSegment> FindPaths()
		{
			string currentScene = agent.Location.scene;
			Vector2 currentPosition = agent.Location.Vector2;
			List<PathSegment> segments = new();

			List<string> scenePath = FindScenePath(currentScene, target.scene);
			if (scenePath == null) return null;
			scenePath.Insert(0, currentScene);

			// For each scene, find the path from the scene entry (or start pos, if it's
			// the first scene) to the scene exit (or target pos, if it's the last scene).
			for (int i = 0; i < scenePath.Count; i++)
			{
				Vector2 localTargetPosition;
				ScenePortal targetPortal = null;

				currentScene = scenePath[i];

				ISet<Vector2Int> blockedInCurrentScene =
					blockedTiles.TryGetValue(currentScene, out var tiles) ? tiles : null;

				if (i == scenePath.Count - 1)
				{
					// This is the last scene.
					localTargetPosition = target.Vector2;
				}
				else
				{
					// Target is the portal to the next scene.
					if (TryFindSceneEntryLocation(
						currentScene,
						scenePath[i + 1],
						blockedInCurrentScene,
						out ScenePortal portal,
						out Vector2 targetLocation))
					{
						localTargetPosition = targetLocation;
						targetPortal = portal;
					}
					else
					{
						// No path to scene entry.
						return null;
					}
				}

				IList<Vector2Int> path = Pathfinder.FindPath(
					currentPosition,
					localTargetPosition,
					currentScene,
					blockedInCurrentScene);

				if (path == null) return null;

				// We've found the path to the local target.
				IList<Vector2> exactPath = TileLocationsToTileCenters(path);

				// Add the exact position in the destination tile as a final step.
				if (i == scenePath.Count - 1) exactPath.Add(target.Vector2);

				segments.Add(new PathSegment { path = exactPath, portal = targetPortal });

				// If this isn't the last scene, update current position to the portal exit.
				if (i < scenePath.Count - 1) currentPosition = targetPortal!.PortalExitSceneCoords;
			}

			return segments;
		}

		private static List<Vector2> TileLocationsToTileCenters(IEnumerable<Vector2Int> tiles)
		{
			return tiles.Select(tile => new Vector2(tile.x + 0.5f, tile.y + 0.5f)).ToList();
		}


		/// Locates a portal between the given scenes and a position from which that portal can be accessed.
		/// Won't navigate through any tiles in the given blacklist in the current scene.
		/// Returns false if no portal exists or the portal is blocked.
		private static bool TryFindSceneEntryLocation(
			string currentScene,
			string targetScene,
			ISet<Vector2Int> tileBlacklist,
			out ScenePortal portal,
			out Vector2 accessPointScenePos)
		{
			portal = null;
			accessPointScenePos = Vector2.zero;

			List<ScenePortal> availablePortals = ScenePortalLibrary.GetPortalsBetweenScenes(currentScene, targetScene);

			if (availablePortals.Count == 0)
			{
				Debug.LogWarning(
					"No scene portal found between scenes \"" + currentScene + "\" and \"" + targetScene + "\".");
				return false;
			}

			// Just pick the first available portal. TODO choose the closest to the agent.
			ScenePortal targetPortal = availablePortals[0];

			// Find accessible entry tiles for the chosen portal.
			HashSet<Vector2Int> possibleLocations = Pathfinder.GetValidAdjacentTiles(
				currentScene,
				TilemapInterface.WorldPosToScenePos(targetPortal.transform.position, targetPortal.PortalScene),
				tileBlacklist);

			if (possibleLocations.Count == 0)
			{
				// Scene portal is blocked.
				Debug.LogWarning("Scene portal is blocked.", targetPortal);
				return false;
			}

			portal = targetPortal;
			accessPointScenePos = possibleLocations.PickRandom(); // TODO pick closest access point instead of any access point.
			return true;
		}

		/// Uses GetAdjacentScenes to find a path (BFS) to the given scene.
		/// Returns the list of scenes to traverse, excluding the starting scene.
		/// Returns null if no path exists.
		private static List<string> FindScenePath(string startScene, string endScene)
		{
			List<string> path = new();
			if (startScene == endScene) return path;

			HashSet<string> visited = new();
			Queue<string> toVisit = new();
			Dictionary<string, string> parent = new();

			toVisit.Enqueue(startScene);
			visited.Add(startScene);

			while (toVisit.Count > 0)
			{
				string currentScene = toVisit.Dequeue();

				foreach (string adjacentScene in ScenePortalLibrary.GetAdjacentScenes(currentScene)
					.Where(adjacentScene => !visited.Contains(adjacentScene)))
				{
					visited.Add(adjacentScene);
					parent[adjacentScene] = currentScene;

					if (adjacentScene == endScene)
					{
						// Found the path.
						path.Add(adjacentScene);
						while (currentScene != startScene)
						{
							path.Add(currentScene);
							currentScene = parent[currentScene];
						}

						path.Reverse();

						Debug.Assert(path.Count > 0);
						Debug.Assert(path.Count == 0 || path[^1] == endScene);

						return path;
					}

					toVisit.Enqueue(adjacentScene);
				}
			}

			return null;
		}
	}
}
