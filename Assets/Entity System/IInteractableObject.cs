using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// PlayerInteractionRaycaster looks for objects of this type;
// PlayerInteractionManager calls the OnPlayerInteract event when the player activates one,
// and the OnInteract method for the relevant object

public interface IInteractableObject {

	void OnInteract ();
}
