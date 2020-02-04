using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigateBehaviour : IAiBehaviour
{
	NPC npc;
	NPCNavigator nav;
	TileLocation destination;
	NPCActivityExecutor.ExecutionCallbackFailable callback;

	Coroutine coroutineObject = null;

	bool isWaitingForNavigationToFinish = false;
	bool isRunning = false;

	public NavigateBehaviour(NPC npc, TileLocation destination, NPCActivityExecutor.ExecutionCallbackFailable callback)
	{
		this.npc = npc;
		nav = npc.GetComponent<NPCNavigator>();
		this.destination = destination;
		this.callback = callback;
	}

	public void Execute()
	{
		isRunning = true;
		coroutineObject = npc.StartCoroutine(TravelCoroutine(destination, callback));
	}
	public void Cancel()
	{
		npc.StopCoroutine(coroutineObject);
		nav.CancelNavigation();
		isWaitingForNavigationToFinish = false;
		isRunning = false;
		callback?.Invoke(false);
	}
	public bool IsRunning => isRunning;

	void OnNavigationFinished ()
	{
		isWaitingForNavigationToFinish = false;
	}

	IEnumerator TravelCoroutine(TileLocation destination, NPCActivityExecutor.ExecutionCallbackFailable callback)
	{
		nav.CancelNavigation();
		if (destination.Scene != npc.CurrentScene)
		{
			// Find a portal to traverse scenes
			// TODO not have every NPC use the same portal every time (take the closest one instead)
			ScenePortal targetPortal = ScenePortalLibrary.GetPortalsBetweenScenes(npc.GetComponent<NPC>().CurrentScene, destination.Scene)[0];
			if (targetPortal == null)
			{
				Debug.LogWarning("Cross-scene navigation failed; no suitable scene portal exists!");
				callback?.Invoke(false);
				yield break;
			}
			Vector2 targetLocation = TileNavigationHelper.GetValidAdjacentTiles(npc.CurrentScene, TilemapInterface.WorldPosToScenePos(targetPortal.transform.position, targetPortal.gameObject.scene.name))[0];

			isWaitingForNavigationToFinish = true;
			nav.FollowPath(TileNavigationHelper.FindPath(npc.transform.localPosition, targetLocation, npc.CurrentScene), npc.CurrentScene, OnNavigationFinished);

			while (isWaitingForNavigationToFinish)
			{
				yield return null;
			}

			// Turn towards scene portal
			nav.ForceDirection(TileNavigationHelper.GetDirectionToLocation(npc.transform.position, targetPortal.transform.position));
			// Pause for a sec
			yield return new WaitForSeconds(0.3f);
			// Activate portal
			npc.GetComponent<NPCActivityExecutor>().ActivateScenePortal(targetPortal);

			// Finish navigation
			isWaitingForNavigationToFinish = true;
			nav.FollowPath(TileNavigationHelper.FindPath(
				TilemapInterface.WorldPosToScenePos(npc.transform.position, npc.CurrentScene),
				destination.Position,
				npc.CurrentScene
			), npc.CurrentScene, OnNavigationFinished);
			
			while (isWaitingForNavigationToFinish)
			{
				yield return null;
			}
		}
		else
		{
			// Destination is on same scene
			isWaitingForNavigationToFinish = true;
			nav.FollowPath(TileNavigationHelper.FindPath(TilemapInterface.WorldPosToScenePos(npc.transform.position, npc.CurrentScene), new Vector2(destination.x, destination.y), npc.CurrentScene), npc.CurrentScene, OnNavigationFinished);
			while (isWaitingForNavigationToFinish)
			{
				yield return null;
			}
		}
		callback?.Invoke(true);
	}
}
