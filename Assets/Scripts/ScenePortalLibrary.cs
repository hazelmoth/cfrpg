using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Stores references to all existing scene portals, for Actor navigation purposes
public static class ScenePortalLibrary
{
	public static List<ScenePortal> Library { get; private set; }

	public static void BuildLibrary () {
		Library = new List<ScenePortal> ();
		foreach (ScenePortal portal in GameObject.FindObjectsOfType<ScenePortal>()) {
			Library.Add (portal);
		}
	}
	public static List<ScenePortal> GetPortalsBetweenScenes (string startScene, string destScene)
	{
		List<ScenePortal> results = new List<ScenePortal> ();
		foreach (ScenePortal portal in Library)
		{
			if (SceneObjectManager.GetSceneIdForObject(portal.gameObject) == startScene && portal.DestinationSceneObjectId == destScene)
			{
				results.Add(portal);
			}
		}
		return results;
	}
	public static List<SerializableScenePortal> GetAllPortalDatas()
	{
		List<SerializableScenePortal> retVal = new List<SerializableScenePortal>();
		foreach (ScenePortal portal in Library)
		{
			if (portal == null || portal.gameObject == null) continue;
			SerializableScenePortal data = portal.GetData();
			retVal.Add(data);
		}
		return retVal;
	}

	/// Returns all the scenes that are directly connected to the given scene by a portal.
	public static List<string> GetAdjacentScenes(string scene)
	{
		HashSet<string> results = new();
		foreach (ScenePortal portal in Library.Where(
			portal => portal.PortalScene == scene))
		{
			results.Add(portal.DestinationSceneObjectId);
		}
		return results.ToList();
	}
}
