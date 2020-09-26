using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls the specific movements of Actors.
// This class should be accessed only by ActorNavigationController.
public class ActorMovementController : MonoBehaviour {
	private const int PIXELS_PER_UNIT = 16;
	private const bool DO_PIXEL_PERFECT_CLAMP = false;
	private const bool CLAMP_TO_SUB_PIXELS = true;

	private ActorAnimController animController;
	private Rigidbody2D  rigidbody;
	private bool isWalking;
	// The speed and direction we're moving
	private Vector2 currentMovement;

	private float speed = 2f;

	// Use this for initialization
	private void Awake () {
		animController = GetComponent<ActorAnimController> ();
		rigidbody = GetComponent<Rigidbody2D> ();
	}

	private void FixedUpdate () {
		Vector3 pos = transform.position;
		Vector3 offset = currentMovement * speed * Time.fixedDeltaTime;
        if (DO_PIXEL_PERFECT_CLAMP)
        {
			pos = PixelPerfectClamp(pos);
			offset = PixelPerfectClamp(offset);
        }
		rigidbody.MovePosition(new Vector3 (pos.x + offset.x, pos.y + offset.y));
	}

	public void SetWalking (Vector2 velocity)
	{
		currentMovement = velocity;
		isWalking = velocity.magnitude > 0f;
		animController.SetWalking (isWalking);
		if (isWalking)
		{
			animController.SetDirection(velocity.ToDirection());
		}
	}
	public void ForceDirection (Direction direction) {
		animController.SetDirection (direction);
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
