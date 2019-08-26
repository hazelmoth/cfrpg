using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SavedPlayerChar
{
	public PlayerCharData data;
	public Vector2 location;
	public Direction direction;
	public string scene;

	public SavedPlayerChar(PlayerCharData data, Vector2 location, Direction direction, string scene)
	{
		this.data = data;
		this.location = location;
		this.direction = direction;
		this.scene = scene;
	}
}
