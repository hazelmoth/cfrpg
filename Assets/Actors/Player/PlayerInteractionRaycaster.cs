using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerInteractionRaycaster : MonoBehaviour 
{
	private Actor player;
	private const float RaycastDistance = 1f;

	private void Start()
	{
		PlayerController.OnPlayerIdSet += GetPlayer;
	}

	public GameObject DetectInteractableObject () 
	{
		if (player == null)
		{
			GetPlayer();
		}

		if (player == null)
		{
			Debug.LogError("No anim controller found on player");
			return null;
		}
		Vector2 direction = player.Direction.ToVector2();
		RaycastHit2D hit = Physics2D.Raycast(player.transform.position, direction, RaycastDistance, ~ (1 << 8));

		Debug.DrawRay (player.transform.position, direction * RaycastDistance, Color.green, Time.deltaTime, false);
		if (hit.collider != null && hit.collider.GetComponent<InteractableObject>() != null) {
			return hit.collider.gameObject;
		}
		return null;
	}

	private void GetPlayer()
	{
		player = ActorRegistry.Get(PlayerController.PlayerActorId).gameObject;
	}
}
