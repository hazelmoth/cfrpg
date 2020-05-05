using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerInteractionRaycaster : MonoBehaviour 
{
	private ActorAnimController playerAnimController;
	private const float RaycastDistance = 1f;

	private void Start()
	{
		PlayerController.OnPlayerIdSet += SetAnimController;
	}

	public GameObject DetectInteractableObject () 
	{
		if (playerAnimController == null)
		{
			SetAnimController();
		}

		if (playerAnimController == null)
		{
			Debug.LogError("No anim controller found on player");
			return null;
		}
		Vector2 direction = playerAnimController.GetDirectionVector2 ();
		RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, RaycastDistance, ~ (1 << (8)));
		Debug.DrawRay (transform.position, direction * RaycastDistance, Color.green, Time.deltaTime, false);
		if (hit.collider != null && hit.collider.GetComponent<InteractableObject>() != null) {
			return hit.collider.gameObject;
		}
		return null;
	}

	private void SetAnimController()
	{
		playerAnimController = ActorRegistry.Get(PlayerController.PlayerActorId).gameObject.GetComponent<ActorAnimController>();
	}
}
