using UnityEngine;

public enum Direction {
	Down, Up, Left, Right
};

public static class DirectionMethods {
	public static Vector2 ToVector2(this Direction direction) {
		switch(direction) {
		case Direction.Down:
			return new Vector2 (0, -1);
		case Direction.Right:
			return new Vector2 (1, 0);
		case Direction.Up:
			return new Vector2 (0, 1);
		case Direction.Left:
		default:
			return new Vector2 (-1, 0);
		}
	}
	public static Direction ToDirection(this Vector2 vector) {
		float x = vector.x;
		float y = vector.y;
		if ((Mathf.Abs(x) < Mathf.Abs(y) && y <= 0) || (x == 0 && y == 0))
			return Direction.Down;
		else if (Mathf.Abs(x) < Mathf.Abs(y) && y > 0)
			return Direction.Up;
		else if (x >= 0)
			return Direction.Right;
		else
			return Direction.Left;
	}
	public static Direction ToDirection(this Vector2Int vector)
	{
		float x = vector.x;
		float y = vector.y;
		if ((Mathf.Abs(x) < Mathf.Abs(y) && y <= 0) || (x == 0 && y == 0))
			return Direction.Down;
		else if (Mathf.Abs(x) < Mathf.Abs(y) && y > 0)
			return Direction.Up;
		else if (x >= 0)
			return Direction.Right;
		else
			return Direction.Left;
	}
	public static Direction Invert(this Direction direction) {
		switch(direction) {
		case Direction.Down:
			return Direction.Up;
		case Direction.Right:
			return Direction.Left;
		case Direction.Up:
			return Direction.Down;
		case Direction.Left:
		default:
			return Direction.Right;
		}
	}

	public static Direction GetRandom ()
	{
		int val = Random.Range(0, 4);
		switch (val)
		{
			case 0:
				return Direction.Up;
			case 1:
				return Direction.Left;
			case 2:
				return Direction.Down;
			default:
				return Direction.Right;
		}
	}

	public static Direction AngleToDir(float angle)
	{
		Vector2 vector = Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.right;
		return vector.ToDirection();
	}
}