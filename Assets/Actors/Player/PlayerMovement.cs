using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	const int PIXELS_PER_UNIT = 16;
	const bool DO_PIXEL_PERFECT_CLAMP = true;
	const bool CLAMP_TO_SUB_PIXELS = true;

	// Takes keyboard input to move the player and calls the animation controller accordingly

	HumanAnimController animController;
	Rigidbody2D rigidbody;
	bool isBlocked;
	public static bool IsBlocked {get{return instance.isBlocked;}}
	static PlayerMovement instance;

	float speed = 3f;

	// Use this for initialization
	void Start () {
		instance = this;
		animController = GetComponent<HumanAnimController> ();
		rigidbody = GetComponent<Rigidbody2D> ();
	}


	void FixedUpdate () {
		if (isBlocked)
			return;
		
		Vector3 pos = transform.position;
		float horizontal = Input.GetAxisRaw ("Horizontal");
		float vertical = Input.GetAxisRaw ("Vertical");

		if (horizontal != 0 || vertical != 0) {
			Vector3 offset = Vector3.ClampMagnitude(new Vector3(horizontal, vertical) * speed * Time.fixedDeltaTime, speed * Time.fixedDeltaTime);

            if (DO_PIXEL_PERFECT_CLAMP)
            {
				pos = PixelPerfectClamp(pos);
				offset = PixelPerfectClamp(offset);
            }

			rigidbody.MovePosition(pos + offset);
			animController.SetWalking (true);

			if (Mathf.Abs(horizontal) < Mathf.Abs(vertical)) {
				if (vertical > 0)
					animController.SetDirection (Direction.Up);
				else
					animController.SetDirection (Direction.Down);
			} else {
				if (horizontal > 0)
					animController.SetDirection (Direction.Right);
				else
					animController.SetDirection (Direction.Left);
			}
		} else {
			animController.SetWalking (false);
		}
	}

	public static void SetMovementBlocked (bool blocked) {
		instance.isBlocked = blocked;
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
