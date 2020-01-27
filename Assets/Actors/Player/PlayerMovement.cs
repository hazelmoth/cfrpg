using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

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
			rigidbody.MovePosition(new Vector3 (pos.x + horizontal * speed * Time.fixedDeltaTime, pos.y + vertical * speed * Time.fixedDeltaTime));
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
}
