using System.Collections.Generic;
using UnityEngine;

// Organizes the portals in and out of an interior when it is placed.
// This script should be on every entity prefab with an interior.
// TODO handle multiple scene portals to an interior
public class InteriorSceneCoordinator : MonoBehaviour, ISaveable
{
	private ScenePortal localPortal;

	string ISaveable.ComponentId => "interior_scene_coordinator";

	// Start is called before the first frame update
	private void Start()
    {
		InitializeInterior();
    }

	private void InitializeInterior ()
	{
		localPortal = GetComponentInChildren<ScenePortal>();
		if (localPortal == null)
		{
			Debug.LogWarning("No scene portal found in children of interior scene coordinator.");
			return;
		}

		GameObject interiorSceneObject;

		string interiorPrefabId = localPortal.DestinationScenePrefabId;

		if (localPortal.DestinationSceneObjectId != null && SceneObjectManager.SceneExists(localPortal.DestinationSceneObjectId))
		{
			// The scene this portal leads to already exists; get the scene object for it
			interiorSceneObject = SceneObjectManager.GetSceneObjectFromId(localPortal.DestinationSceneObjectId);
		}
		else
		{
			// This scene for this object does not exist. Create a new one.
			string interiorObjectId = SceneObjectManager.CreateNewSceneFromPrefab(interiorPrefabId);
			localPortal.SetExitSceneObjectId(interiorObjectId);
			interiorSceneObject = SceneObjectManager.GetSceneObjectFromId(interiorObjectId);
		}

		// TODO handle multiple exit portals
		ScenePortal destinationPortal = interiorSceneObject.GetComponentInChildren<ScenePortal>();

		if (destinationPortal == null)
		{
			Debug.LogWarning("Initializing an interior with no exit portal!", interiorSceneObject);
			return;
		}

		destinationPortal.SetExitSceneObjectId(SceneObjectManager.GetSceneIdForObject(this.gameObject));

		// Set the exit coords of each portal to be next to the other portal, facing the opposite way
		Vector2 localExitCoords = TilemapInterface.WorldPosToScenePos(destinationPortal.transform.position, SceneObjectManager.GetSceneIdForObject(destinationPortal.gameObject));
		localExitCoords += destinationPortal.EntryDirection.Invert().ToVector2();
		localPortal.SetExitCoords(localExitCoords);

		Vector2 destExitCoords = TilemapInterface.WorldPosToScenePos(localPortal.transform.position, SceneObjectManager.GetSceneIdForObject(localPortal.gameObject));
		destExitCoords += localPortal.EntryDirection.Invert().ToVector2();
		destinationPortal.SetExitCoords(destExitCoords);

		ScenePortalLibrary.BuildLibrary();
	}

	IDictionary<string, string> ISaveable.GetTags()
	{
		// Currently saving isn't working because this class, when started, creates
		// a scene if it doesn't exist, but the save loader then tries to load its
		// saved version of that scene... idk man, maybe we don't need this class
		// to have save tags
		return new Dictionary<string, string>();
		
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
		InitializeInterior();
	}
}
