using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Defines an object which actors can interact with continuously
public interface IContinuouslyInteractable
{
	// Should be called every frame that an actor uses this object.
	void Interact();
}
