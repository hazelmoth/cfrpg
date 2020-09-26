using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Stores the data of all the entities available to be placed
public class EntityLibraryObject : ScriptableObject
{
	// IDs and their respective entities; these two lists must always be the same length
	public List<string> libraryIds;
	public List<EntityData> libraryEntities;
}
