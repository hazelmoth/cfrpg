using System.Collections.Generic;
using UnityEngine;

public static class VectorExtension
{
	public static Vector2Int ToVector2Int(this Vector2 vector2) {
		return Vector2Int.FloorToInt (vector2);
	}
	public static Vector2Int ToVector2Int(this Vector3 vector3)
	{
		return Vector2Int.FloorToInt(vector3);
	}
	public static Vector2Int ClosestFromList(this Vector2Int vector2Int, List<Vector2Int> list)
	{
		if (list.Count == 0)
		{
			Debug.LogError("List is empty!");
			return vector2Int;
		}
		Vector2Int closest = list[0];
		float closestDistance = Vector2Int.Distance(vector2Int, closest);
		for (int i = 1; i < list.Count; i++)
		{
			float thisDistance = Vector2Int.Distance(vector2Int, list[i]);
			if (thisDistance < closestDistance)
			{
				closest = list[i];
				closestDistance = thisDistance;
			}
		}
		return closest;
	}
	public static Vector3Int ToVector3Int(this Vector3 vector3)
	{
		return Vector3Int.FloorToInt(vector3);
	}
	public static Vector2 ToVector2 (this Vector3 vector3)
	{
		return new Vector2(vector3.x, vector3.y);
	}
	public static Vector2 ToVector2(this Vector3Int vector3Int)
	{
		return new Vector2(vector3Int.x, vector3Int.y);
	}
	public static Vector2 ClosestFromList(this Vector2 vector2, List<Vector2> list)
	{
		if (list.Count == 0)
		{
			Debug.LogError("List is empty!");
			return vector2;
		}
		Vector2 closest = list[0];
		float closestDistance = Vector2.Distance(vector2, closest);
		for (int i = 1; i < list.Count; i++)
		{
			float thisDistance = Vector2.Distance(vector2, list[i]);
			if (thisDistance < closestDistance)
			{
				closest = list[i];
				closestDistance = thisDistance;
			}
		}
		return closest;
	}
	public static Vector2IntSerializable ToSerializable(this Vector2Int vector2Int)
	{
		return new Vector2IntSerializable(vector2Int.x, vector2Int.y);
	}
	public static Vector2Int ToVector2Int(this Vector2IntSerializable vector2IntSerializable)
	{
		return new Vector2Int(vector2IntSerializable.x, vector2IntSerializable.y);
	}
}

[System.Serializable]
public struct Vector2IntSerializable
{
	public int x;
	public int y;

	public Vector2IntSerializable(int x, int y)
	{
		this.x = x;
		this.y = y;
	}
}
