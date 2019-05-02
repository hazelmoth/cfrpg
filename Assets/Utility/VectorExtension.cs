using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorExtension
{
	public static Vector2Int ToVector2Int(this Vector2 vector2) {
		return Vector2Int.FloorToInt (vector2);
	}
	public static Vector2 ToVector2 (this Vector3 vector3)
	{
		return new Vector2(vector3.x, vector3.y);
	}
	public static Vector2 ToVector2(this Vector3Int vector3Int)
	{
		return new Vector2(vector3Int.x, vector3Int.y);
	}
}
