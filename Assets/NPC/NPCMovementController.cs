using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls the specific movements of NPCs.
// This class should be accessed only by NPCNavigationController.
public class NPCMovementController : MonoBehaviour {

	HumanAnimController animController;
	Rigidbody2D rigidbody;
	Direction currentDirection;
	bool isWalking;
	// The speed and direction we're moving
	Vector2 currentMovement;

	float speed = 2f;

	// Use this for initialization
	void Start () {
		animController = GetComponent<HumanAnimController> ();
		rigidbody = GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 pos = transform.position;
		rigidbody.MovePosition(new Vector3 (pos.x + currentMovement.x * speed * Time.deltaTime, pos.y + currentMovement.y * speed * Time.deltaTime));
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
}
