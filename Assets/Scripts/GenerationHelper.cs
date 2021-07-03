using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GenerationHelper
{
	public static float Avg (float a, float b)
	{
		return (a + b) / 2;
	}

	// A noise function with a somewhat uniform distribution
	public static float UniformSimplex(float x, float y, float seed)
	{
		SimplexNoiseGenerator noise = new SimplexNoiseGenerator(seed.ToString());

		float n = noise.noise(x, y, 0);
		// Rescale the noise so we're only sampling a third of its original range
		n *= 3f;
		n += 0.5f;
		n = Mathf.Clamp01(n);
		return n;
	}

	// Returns a value from 0 to 1 based on the location of a point on a linear
	// gradient. Gradient is on the given axis, starting from the given value on
	// that axis and rising to the given end value.
	public static float LinearGradient(Vector2 point, bool horizontal, int start, int end)
	{
		return Mathf.Clamp01(
			((horizontal ? point.x : point.y) - start) / (end - start));
	}

	// Returns a value between 0 and 1 based on where a point is between the origin and a surrounding ellipse.
	// 1 is the center of the ellipse, 0 is the outside. The ellipse is centered at the origin.
	public static float EllipseGradient(Vector2 point, float width, float height)
	{
		// diameters to radii
		width = width / 2;
		height = height / 2;

		if (point.x == 0 && point.y == 0)
		{
			return 1;
		}
		// Find point on ellipse that is on the line between the origin and input point
		float x = Mathf.Sqrt(Mathf.Pow(width * height * point.x, 2f) / (Mathf.Pow(point.x * height, 2) + Mathf.Pow(point.y * width, 2)));
		float y = height * Mathf.Sqrt(1 - Mathf.Pow(x / width, 2));
		if (point.x < 0)
			x *= -1;
		if (point.y < 0)
			y *= -1;
		Vector2 ellipsePoint = new Vector2(x, y);

		float z = Vector2.Distance(Vector2.zero, point) / Vector2.Distance(Vector2.zero, ellipsePoint);
		z = Mathf.Clamp01(z);
		// Invert so 1 is the center
		z = 1 - z;
		return z;
	}

	// Returns a position along a spiral after the given degrees of rotations, moving out at the given number of units
	// per rotation and starting at the given angle.
	public static Vector2 Spiral(float radiusPerRot, float startAngle, bool clockwise, float degrees)
	{
		float rot = degrees / 360;
		float radius = rot * radiusPerRot;
		float angle = startAngle;
		angle += rot * 360 * (clockwise ? -1f : 1f);
		Vector2 vector = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
		vector *= radius;
		return vector;
	}
}
