using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanAnimController : MonoBehaviour {

	protected Animator animator;
	protected SpriteRenderer renderer;

	// To account for humans' origins being directly under them (unlike tiles)
	readonly public static Vector2 HumanTileOffset = new Vector2 (0.5f, 0.5f); 

	void Start () {
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
