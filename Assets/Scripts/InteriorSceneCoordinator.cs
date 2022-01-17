using System.Collections.Generic;
using UnityEngine;

// Organizes the portals in and out of an interior when it is placed.
// This script should be on every entity prefab with an interior.
// TODO handle multiple scene portals to an interior
public class InteriorSceneCoordinator : MonoBehaviour, ISaveable
{
	private ScenePortal localPortal;
	private bool initialized;

	string ISaveable.ComponentId => "interior_scene_coordinator";

	private void Update()
	{
		if (initialized) return;

		localPortal = GetComponentInChildren<ScenePortal>();
		if (localPortal == null)
		{
			Debug.LogWarning("No scene portal found in children of interior scene coordinator.");
			return;
		}

		// If this portal links to an existing scene, delay initialization until that
		// scene has been loaded.
		if (localPortal.DestinationSceneObjectId != null
			&& !SceneObjectManager.SceneExists(localPortal.DestinationSceneObjectId)) return;

		InitializeInterior();
		initialized = true;
	}

	/// Returns the scene portal that links to the interior.
	public ScenePortal GetEntrancePortal()
	{
		return localPortal;
	}

	/// Find the interior scene this portal goes to (or create one if it doesn't exist)
	/// and then link this portal to the interior's portal.
	private void InitializeInterior ()
	{
		GameObject interiorSceneObject;

		if (localPortal.DestinationSceneObjectId != null)
		{
			if (!SceneObjectManager.SceneExists(localPortal.DestinationSceneObjectId))
			{
				// This portal links to a scene, but that scene isn't loaded.
				// This is a failure.
				Debug.LogError($"Failed to initialize interior; scene portal links to a scene "
					+ $"\"{localPortal.DestinationSceneObjectId}\" that isn't loaded.");
			}
			// The scene this portal leads to already exists; grab the scene object for it.
			interiorSceneObject = SceneObjectManager.GetSceneObjectFromId(localPortal.DestinationSceneObjectId);
		}
		else
		{
			// This portal doesn't link to any scene; let's initialize one from the prefab.
			string interiorPrefabId = localPortal.DestinationScenePrefabId;
			string interiorSceneId = SceneObjectManager.CreateNewSceneFromPrefab(interiorPrefabId);
			localPortal.SetExitSceneObjectId(interiorSceneId);
			interiorSceneObject = SceneObjectManager.GetSceneObjectFromId(interiorSceneId);
		}

		// TODO handle multiple exit portals
		ScenePortal destinationPortal = interiorSceneObject.GetComponentInChildren<ScenePortal>();

		if (destinationPortal == null)
		{
			Debug.LogWarning("Initializing an interior with no exit portal!", interiorSceneObject);
			return;
		}

		destinationPortal.SetExitSceneObjectId(SceneObjectManager.GetSceneIdForObject(this.gameObject));

		// Set the exit coords of each portal to be next to the other portal, facing the opposite way.

		Vector2 localExitCoords = TilemapInterface.WorldPosToScenePos(
			destinationPortal.transform.position,
			SceneObjectManager.GetSceneIdForObject(destinationPortal.gameObject));

		localExitCoords += destinationPortal.EntryDirection.Invert().ToVector2();
		localPortal.SetExitCoords(localExitCoords);

		Vector2 destExitCoords = TilemapInterface.WorldPosToScenePos(
			localPortal.transform.position,
			SceneObjectManager.GetSceneIdForObject(localPortal.gameObject));

		destExitCoords += localPortal.EntryDirection.Invert().ToVector2();
		destinationPortal.SetExitCoords(destExitCoords);

		ScenePortalLibrary.BuildLibrary();
	}

	IDictionary<string, string> ISaveable.GetTags()
	{
		if (localPortal == null)
		{
			localPortal = GetComponentInChildren<ScenePortal>();
		}
		if (localPortal == null)
		{
			Debug.LogWarning("No scene portal found in children of interior scene coordinator.");
			return null;
		}

		string interiorSceneId = localPortal.DestinationSceneObjectId;
		Dictionary<string, string> tags = new Dictionary<string, string>();
		tags["interiorSceneId"] = interiorSceneId;
		return tags;
	}

	void ISaveable.SetTags(IDictionary<string, string> tags)
	{
		if (initialized)
			Debug.LogError("Save tags, if any, should be set before this class initializes!");

		if (tags.Count < 1)
		{
			Debug.LogError("Component tags set without enough tags");
			return;
		}

		if (localPortal == null)
		{
			localPortal = GetComponentInChildren<ScenePortal>();
		}
		if (localPortal == null)
		{
			Debug.LogWarning("No scene portal found in children of interior scene coordinator.");
			return;
		}

		// Set the interior scene for the portal
		localPortal.SetExitSceneObjectId(tags["interiorSceneId"]);
	}
}
