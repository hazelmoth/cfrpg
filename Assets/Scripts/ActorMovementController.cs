using UnityEngine;

// Controls the specific movements of Actors.
public class ActorMovementController : MonoBehaviour {
	
	private const int PixelsPerUnit = 16;
	private const bool DoPixelPerfectClamp = false;
	private const bool ClampToSubPixels = true;
	
	private const float KnockbackTime = 0.1f; // How long a knockback takes
	private const float Speed = 3f; // Target movement speed in units/sec

	private ActorAnimController animController;
	private new Rigidbody2D rigidbody;
	private bool isWalking;
	private Vector2 currentMovement; // The speed and direction we're moving
	private Vector2 currentKnockback; // The knockback movement vector, if one is active
	private float knockbackStart = -100f; // When the last knockback began

	// Use this for initialization
	private void Awake () {
		animController = GetComponent<ActorAnimController> ();
		rigidbody = GetComponent<Rigidbody2D> ();
	}

	private void FixedUpdate () {
		Vector3 pos = transform.position;
		Vector3 offset = currentMovement * (Speed * Time.fixedDeltaTime);
        if (DoPixelPerfectClamp)
        {
			pos = PixelPerfectClamp(pos);
			offset = PixelPerfectClamp(offset);
        }
        Vector3 targetPosition = new Vector3(pos.x + offset.x, pos.y + offset.y);
		if (Time.time - knockbackStart < KnockbackTime)
		{
			targetPosition = pos + (Vector3) currentKnockback * ((Time.fixedDeltaTime / KnockbackTime) *
			                                                     (2 - 2 * ((Time.time - knockbackStart) /
			                                                               KnockbackTime)));
		}
		
		rigidbody.MovePosition(targetPosition);
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

	public void KnockBack(Vector2 movement)
	{
		currentKnockback = movement;
		knockbackStart = Time.time;
	}

	private static Vector2 PixelPerfectClamp(Vector2 input)
	{
		float pixelSize = 1;

		if (ClampToSubPixels)
		{
			pixelSize = GetPixelSize(Screen.height, Camera.main.orthographicSize);
		}

		Vector2 vectorInSubPixels = new Vector2
		{
			x = Mathf.RoundToInt(input.x * PixelsPerUnit * pixelSize),
			y = Mathf.RoundToInt(input.y * PixelsPerUnit * pixelSize)
		};

		return vectorInSubPixels / (PixelsPerUnit * pixelSize);
	}

	// Returns the width of a game pixel in real screen pixels
	private static float GetPixelSize(int resY, float camSize)
	{
		float result = resY / (camSize * 2 * PixelsPerUnit);
		return result;
	}
}
