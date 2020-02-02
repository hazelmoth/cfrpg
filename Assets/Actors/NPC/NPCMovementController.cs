using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls the specific movements of NPCs.
// This class should be accessed only by NPCNavigationController.
public class NPCMovementController : MonoBehaviour {

	const int PIXELS_PER_UNIT = 16;
	const bool DO_PIXEL_PERFECT_CLAMP = false;
	const bool CLAMP_TO_SUB_PIXELS = true;

	HumanAnimController animController;
	Rigidbody2D  rigidbody;
	Direction currentDirection;
	bool isWalking;
	// The speed and direction we're moving
	Vector2 currentMovement;

	float speed = 2f;

	// Use this for initialization
	void Awake () {
		animController = GetComponent<HumanAnimController> ();
		rigidbody = GetComponent<Rigidbody2D> ();
	}
	
	void FixedUpdate () {
		Vector3 pos = transform.position;
		Vector3 offset = currentMovement * speed * Time.fixedDeltaTime;
        if (DO_PIXEL_PERFECT_CLAMP)
        {
			pos = PixelPerfectClamp(pos);
			offset = PixelPerfectClamp(offset);
        }
		rigidbody.MovePosition(new Vector3 (pos.x + offset.x, pos.y + offset.y));
	}

	public void SetWalking (bool walking) {
		isWalking = walking;
		animController.SetWalking (isWalking);
		animController.SetDirection (currentDirection);
		if (walking) {
			currentMovement = animController.GetDirectionVector2 ();
		}
		else {
			currentMovement = Vector2.zero;
		}
	}
	public void SetDirection (Direction direction) {
		currentDirection = direction;
		animController.SetDirection (currentDirection);
		if (isWalking) {
			currentMovement = animController.GetDirectionVector2 ();
		}
	}

	private static Vector2 PixelPerfectClamp(Vector2 input)
	{
		float pixelSize = 1;

		if (CLAMP_TO_SUB_PIXELS)
		{
			pixelSize = GetPixelSize(Screen.height, Camera.main.orthographicSize);
		}

		Vector2 vectorInSubPixels = new Vector2
		{
			x = Mathf.RoundToInt(input.x * PIXELS_PER_UNIT * pixelSize),
			y = Mathf.RoundToInt(input.y * PIXELS_PER_UNIT * pixelSize)
		};

		return vectorInSubPixels / (PIXELS_PER_UNIT * pixelSize);
	}

	// Returns the width of a game pixel in real screen pixels
	private static float GetPixelSize(int resY, float camSize)
	{
		float result = resY / (camSize * 2 * PIXELS_PER_UNIT);
		return result;
	}
}
