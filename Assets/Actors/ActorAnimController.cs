using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Serves as an interface for human body animations.
// The only class on an actor that should interface with the animator.
public class ActorAnimController : MonoBehaviour {

	protected Animator animator;
	protected SpriteRenderer renderer;

	// To account for humans' origins being directly under them (unlike tiles)
	public static readonly Vector2 HumanTileOffset = new Vector2 (0.5f, 0.5f);

	private void Awake () {
		animator = GetComponent<Animator> ();
		renderer = GetComponent<SpriteRenderer> ();
	}

	// Virtual because PlayerAnimController needs to override this
	public virtual void SetDirection (Direction dir) {
		switch(dir) {
		case Direction.Down:
			animator.SetInteger ("direction", 0);
			break;
		case Direction.Right:
			animator.SetInteger ("direction", 1);
			break;
		case Direction.Up:
			animator.SetInteger ("direction", 2);
			break;
		case Direction.Left:
			animator.SetInteger ("direction", 3);
			break;
		}
	}

	public void SetWalking (bool isWalking) {
		animator.SetBool ("isWalking", isWalking);
	}
	public bool IsWalking {
		get {
			return animator.GetBool ("isWalking");
		}
	}

	public void AnimatePunch (float duration, Direction direction)
	{
		StartCoroutine(PunchCoroutine(duration, direction));
	}

	private IEnumerator PunchCoroutine (float duration, Direction direction)
	{
		switch (direction)
		{
			case Direction.Down:
				animator.SetInteger("punchDirection", 0);
				break;
			case Direction.Right:
				animator.SetInteger("punchDirection", 1);
				break;
			case Direction.Up:
				animator.SetInteger("punchDirection", 2);
				break;
			case Direction.Left:
				animator.SetInteger("punchDirection", 3);
				break;
		}
		animator.SetTrigger("startPunch");
		yield return new WaitForSeconds(duration);
		animator.SetTrigger("finishPunch");
	}

	public Direction GetDirection () {
		int dir = animator.GetInteger ("direction");
		switch (dir) {
		case 0:
			return Direction.Down;
		case 1:
			return Direction.Right;
		case 2:
			return Direction.Up;
		default:
			return Direction.Left;
		}
	}
	public Direction GetPunchDirection()
	{
		int dir = animator.GetInteger("punchDirection");
		switch (dir)
		{
			case 0:
				return Direction.Down;
			case 1:
				return Direction.Right;
			case 2:
				return Direction.Up;
			default:
				return Direction.Left;
		}
	}

	public Vector2 GetDirectionVector2 () {
		int dir = animator.GetInteger ("direction");
		switch (dir) {
		case 0:
			return Vector2.down;
		case 1:
			return Vector2.right;
		case 2:
			return Vector2.up;
		default:
			return Vector2.left;
		}
	}
}
