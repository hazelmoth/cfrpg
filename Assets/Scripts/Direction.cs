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

	public static Direction ToDirection(this Vector2 vector) 
	{
		float angle = Vector2.SignedAngle(Vector2.up, vector);

		// All diagonals result in left and right directions, with a 5 degree margin.
		return angle switch
		{
			float value when Mathf.Abs(value) < 40 => Direction.Up,
			float value when value > 0 && value < 140 => Direction.Left,
			float value when value < 0 && value > -140 => Direction.Right,
			_ => Direction.Down
		};
	}

	public static Direction ToDirection(this Vector2Int vector)
	{
		return ((Vector2)vector).ToDirection();
	}

	public static Direction Invert(this Direction direction) 
	{
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