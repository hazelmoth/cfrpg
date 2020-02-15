using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionChecker : MonoBehaviour
{
	public bool Colliding { get; private set; }

	private void OnTriggerEnter2D(Collider2D collider)
	{
		Colliding = true;
	}

	private void OnTriggerExit2D(Collider2D collider)
	{
		Colliding = false;
	}
}
