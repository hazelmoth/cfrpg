using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SavedEntity
{
	public string id;
	public string scene;
	public Vector2IntSerializable location;
	public List<SavedComponentState> components;

	public SavedEntity(string id, string scene, Vector2Int location, List<SavedComponentState> components)
	{
		this.id = id;
		this.scene = scene;
		this.location = location.ToSerializable();
		this.components = components;
	}
}
