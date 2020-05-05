using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhenThePlayerShouldBeAbleToMoveDecider : MonoBehaviour {

	// Use this for initialization
	private void Start () {
		PlayerMovement.SetMovementBlocked (false);
		UIManager.OnOpenDialogueScreen += OnDialogueEnter;
		UIManager.OnExitDialogueScreen += OnDialogueExit;
	}
	
	// Update is called once per frame
	private void Update () {
		
	}

	private void OnDialogueEnter () {
		PlayerMovement.SetMovementBlocked (true);
	}

	private void OnDialogueExit () {
		PlayerMovement.SetMovementBlocked (false);
	}
}
