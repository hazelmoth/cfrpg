using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhenThePlayerShouldBeAbleToMoveDecider : MonoBehaviour {

	// Use this for initialization
	void Start () {
		PlayerMovement.SetMovementBlocked (false);
		UIManager.OnOpenDialogueScreen += OnDialogueEnter;
		UIManager.OnExitDialogueScreen += OnDialogueExit;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnDialogueEnter () {
		PlayerMovement.SetMovementBlocked (true);
	}
	void OnDialogueExit () {
		PlayerMovement.SetMovementBlocked (false);
	}
}
