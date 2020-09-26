using UnityEngine;

// Adjusts the position of this camera so that all screen pixels fit perfectly within game pixels
// (assuming the camera size divides nicely into the screen resolution)
public class PixelPerfectPosition : MonoBehaviour
{
	private const int PIXELS_PER_UNIT = 16;


    // Update is called once per frame
    private void Update()
    {
		float newLocalX = GetClosestSnapPosition(Screen.height, Camera.main.orthographicSize).x - (transform.position.x - transform.localPosition.x);
		float newLocalY = GetClosestSnapPosition(Screen.height, Camera.main.orthographicSize).y - (transform.position.y - transform.localPosition.y);
		this.transform.localPosition = new Vector3 (newLocalX, newLocalY, this.transform.localPosition.z);
    }

	private Vector3 GetClosestSnapPosition (int resY, float camSize)
	{
		float newX;
		float newY;
		// Get the x and y ignoring local transform
		Vector3 actualPos = new Vector3(transform.position.x - transform.localPosition.x, transform.position.y - transform.localPosition.y, transform.position.z);
		float movementIncrement = CalculateIncrement(resY, camSize);

		// Set each new position component to the highest multiple of the increment less than the actual position
		newX = actualPos.x - (actualPos.x % movementIncrement);
		newY = actualPos.y - (actualPos.y % movementIncrement);

		// If either component is closer to the next highest multiple, add the increment
		if (actualPos.x % movementIncrement > movementIncrement / 2)
		{
			newX += movementIncrement;
		}
		if (actualPos.y % movementIncrement > movementIncrement / 2)
		{
			newY += movementIncrement;
		}

		return new Vector3(newX, newY, actualPos.z);
	}

	// How many units the camera should move at a time
	private float CalculateIncrement (int resY, float camSize)
	{
		return (1 / GetPixelSize(resY, camSize)) / PIXELS_PER_UNIT;
	}

	// Returns the width of a game pixel in real screen pixels
	private float GetPixelSize (int resY, float camSize)
	{
		float result = resY / (camSize * 2 * PIXELS_PER_UNIT);
		return result;
	}
}
