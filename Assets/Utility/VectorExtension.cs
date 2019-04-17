using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorExtension
{
	public static Vector2Int ToVector2Int(this Vector2 vector2) {
		return Vector2Int.FloorToInt (vector2);
	}
}
