using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Organizes the portals in and out of an interior when it is placed.
// This script should be on every entity prefab with an interior.
// TODO handle multiple scene portals to an interior
public class InteriorSceneCoordinator : SaveableComponent
{
	ScenePortal localPortal;

	public override string ComponentId => "interior_scene_coordinator";

	// Tags:
	// location relative to scene
	// scene containing portal
	// interior scene prefab
	// interior scene id
	// exit location relative to scene
	// exit direction
	// bool activate on touch
	// bool owned by entity
	public override List<string> Tags
	{ get
		{
			string locationInScene = localPortal.transform.position.ToString("R");
			Debug.Log(locationInScene);
			string interiorSceneId = null;

			if (localPortal == null)
			{
				localPortal = GetComponentInChildren<ScenePortal>();
			}

			if (localPortal != null)
			{
				interiorSceneId = localPortal.DestinationSceneObjectId;
			}
			return new List<string> { interiorSceneId };
		}
	}

	public override void SetTags(List<string> tags)
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
		localPortal.SetExitSceneObjectId(tags[0]);
		InitializeInterior();
	}

	// Start is called before the first frame update
	void Start()
    {
		InitializeInterior();
    }

	void InitializeInterior ()
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
			Debug.LogWarning("Initializing an interior with no exit portal!");
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
}
