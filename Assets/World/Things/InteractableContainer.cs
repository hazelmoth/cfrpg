using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableContainer : MonoBehaviour, InteractableObject {

	[SerializeField] string containerName;
	[SerializeField] int numSlots;
	[SerializeField] Item[] inventory;

	public void OnInteract () {}


	public Item[] GetContainerInventory () {
		return inventory;
	}

	public int NumSlots {
		get {return numSlots;}
	}

	public string ContainerName {
		get {return containerName;}
	}
}
