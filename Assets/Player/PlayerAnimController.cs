using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimController : HumanAnimController {

	// Modifies HumanAnimController to interface with PlayerClothingManager

	public delegate void DirectionChange (Direction dir);
	public static event DirectionChange OnDirectionChange;

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator> ();
		renderer = GetComponent<SpriteRenderer> ();
	}

	public override void SetDirection (Direction dir) {
		base.SetDirection (dir);
		if (OnDirectionChange != null)
			OnDirectionChange (dir);
	}
}
