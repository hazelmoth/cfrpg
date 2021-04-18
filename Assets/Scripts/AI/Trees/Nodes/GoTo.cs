using System.Collections;
using System.Collections.Generic;
using AI.Behaviours;
using UnityEngine;

namespace AI.Trees.Nodes
{
    public class GoTo : Node
    {
	    private const int MaxRetries = 10; // How many times we'll recalculate a blocked
	                                       // route before returning a failure.

	    private Actor agent;
        private Location target;
        private float margin;
        private ActorNavigator nav;
        private IDictionary<string, ISet<Vector2>> blockedTiles;

        private IList<PathSegment> paths; // A path through each scene to the destination
        private int currentSegment; // Which segment of the path will be followed next
        private bool waitingForNavigation;
        private bool lastNavFailed;
        private int failedAttempts;

        public GoTo(Actor agent, Location target, float margin)
        {
            this.agent = agent;
            this.target = target;
            this.margin = margin;
        }
    
        protected override void Init()
        {
            nav = agent.GetComponent<ActorNavigator>();
            blockedTiles = new Dictionary<string, ISet<Vector2>>();
            paths = FindPaths();
        }

        protected override Status OnUpdate()
        {
	        if (target.scene == agent.Location.scene && Vector2.Distance(target.Vector2, agent.Location.Vector2) < margin)
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
		        else return Status.Failure;
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
		        Debug.LogWarning("Navigation result was unsatisfactory. Distance: " +
		                         $"{Vector2.Distance(target.Vector2, agent.Location.Vector2)}; Margin: {margin}", agent);
		        return Status.Failure;
	        }

	        if (waitingForNavigation) return Status.Running;

            if (currentSegment != paths.Count - 1)
            {
	            // Not waiting. Move to the next scene.
	            ScenePortalActivator.Activate(agent, paths[currentSegment].portal);
            }

            // Set the navigator on the next path.
            nav.FollowPath(paths[currentSegment].path, agent.CurrentScene, NavFinished);
            waitingForNavigation = true;
            
            return Status.Running;
        }
        
        private void NavFinished (bool success, Vector2Int obstaclePos)
        {
	        if (!success)
	        {
		        if (!blockedTiles.ContainsKey(agent.CurrentScene)) blockedTiles.Add(agent.CurrentScene, new HashSet<Vector2>());
		        blockedTiles[agent.CurrentScene].Add(obstaclePos);
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

        // Finds paths through the scenes leading to the destination. Currently
        // can only handle a single scene portal to the target.
        private IList<PathSegment> FindPaths()
        {
	        IList<PathSegment> paths = new List<PathSegment>();
	        string currentScene = agent.Location.scene;
	        Vector2 currentPosition = agent.Location.Vector2;
	        
	        if (target.scene != agent.CurrentScene)
	        {
		        // Find a portal to traverse scenes
		        
		        ISet<Vector2> blockedInCurrentScene =
			        blockedTiles.TryGetValue(agent.CurrentScene, out var tiles) ? tiles : null;

		        if (!TryFindSceneEntryLocation(agent.CurrentScene, target.scene, blockedInCurrentScene,
			        out ScenePortal targetPortal, out Vector2 targetLocation))
		        {
			        return null;
		        }

		        IList<Vector2> path = Pathfinder.FindPath(
			        agent.transform.localPosition,
			        targetLocation,
			        agent.CurrentScene,
			        blockedInCurrentScene);

		        if (path == null)
		        {
			        // No path to scene entry.
			        return null;
		        }
		        
		        // We've found the path to the portal.
		        paths.Add(new PathSegment {path = path, portal = targetPortal});
		        currentScene = targetPortal.DestinationSceneObjectId;
		        currentPosition = targetPortal.PortalExitSceneCoords;
	        }
	        Debug.Assert(currentScene == target.scene);
	        
	        // Now find the path within destination scene.
	        IList<Vector2> finalPath = Pathfinder.FindPath(
		        currentPosition,
		        target.Vector2,
		        currentScene,
		        blockedTiles.TryGetValue(target.scene, out var tiles2) ? tiles2 : null);
	        if (finalPath == null) return null;
	        paths.Add(new PathSegment {path = finalPath, portal = null});

	        return paths;
        }
        
        
        // Locates a portal between the given scenes and a position from which that portal can be accessed.
        // Won't navigate through any tiles in the given blacklist in the current scene.
        // Returns false if no portal exists or the portal is blocked.
        private static bool TryFindSceneEntryLocation (string currentScene, string targetScene, ISet<Vector2> tileBlacklist, out ScenePortal portal, out Vector2 accessPoint)
        {
	        portal = null;
	        accessPoint = Vector2.zero;

	        List<ScenePortal> availablePortals = ScenePortalLibrary.GetPortalsBetweenScenes(currentScene, targetScene);

	        if (availablePortals.Count == 0)
	        {
		        Debug.LogWarning("No scene portal found between scenes \"" + currentScene + "\" and \"" + targetScene + "\".");
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
	        accessPoint = possibleLocations.PickRandom(); // TODO pick closest access point instead of any access point.
	        return true;
        }
    }
}
