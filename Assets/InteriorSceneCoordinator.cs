using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Organizes the portals in and out of an interior when it is placed.
// This script should be on every entity prefab with an interior.
public class InteriorSceneCoordinator : MonoBehaviour
{
	ScenePortal localPortal;
    // Start is called before the first frame update
    void Start()
    {
		localPortal = GetComponentInChildren<ScenePortal> ();
		if (localPortal == null) {
			return;
		}

		string interiorPrefabId = localPortal.DestinationScenePrefabId;
		string interiorObjectId = SceneObjectManager.CreateNewScene (localPortal.DestinationScenePrefabId);
		localPortal.SetExitSceneObjectId(interiorObjectId);

		GameObject interiorSceneObject = SceneObjectManager.GetSceneObjectFromId (interiorObjectId);
		ScenePortal destinationPortal = interiorSceneObject.GetComponentInChildren<ScenePortal> ();
		if (destinationPortal == null) {
			Debug.LogWarning ("Creating an interior with no exit portal!");
			return;
		}
		destinationPortal.SetExitSceneObjectId (SceneObjectManager.GetSceneIdForObject (this.gameObject));

		// Set the exit coords of each portal to be next to the other portal, facing the opposite way
		Vector2 localExitCoords = TilemapInterface.WorldPosToScenePos(destinationPortal.transform.position, SceneObjectManager.GetSceneIdForObject (destinationPortal.gameObject));
		localExitCoords += destinationPortal.EntryDirection.Invert ().ToVector2();
		Debug.Log (destinationPortal.EntryDirection.Invert ().ToVector2 ());
		Debug.Log (localExitCoords);
		localPortal.SetExitCoords (localExitCoords);

		Vector2 destExitCoords = TilemapInterface.WorldPosToScenePos(localPortal.transform.position, SceneObjectManager.GetSceneIdForObject (localPortal.gameObject));
		Debug.Log (destExitCoords);
		destExitCoords += localPortal.EntryDirection.Invert ().ToVector2();
		Debug.Log (destExitCoords);
		destinationPortal.SetExitCoords (destExitCoords);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
