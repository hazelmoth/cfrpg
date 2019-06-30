using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerInteractionRaycaster : MonoBehaviour {

	HumanAnimController animController;
	const float raycastDistance = 1f;
	// Use this for initialization
	void Start () {
		animController = GetComponent<HumanAnimController> ();
	}
	
	// Update is called once per frame
	void Update () {
	}

	public GameObject DetectInteractableObject () {
		if (animController == null)
			animController = GetComponent<HumanAnimController>();
		if (animController == null)
		{
			Debug.LogError("No anim controller found on player");
			return null;
		}
		Vector2 direction = animController.GetDirectionVector2 ();
		RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, raycastDistance, ~ (1 << (8)));
		Debug.DrawRay (transform.position, direction * raycastDistance, Color.green, Time.deltaTime, false);
		if (hit.collider != null && hit.collider.GetComponent<InteractableObject>() != null) {
			return hit.collider.gameObject;

		}
		return null;
	}

	public TileBase DetectTile () {
		if (DetectInteractableObject () != null)
			return null;
		return TilemapInterface.GetTileAtPosition (animController.GetDirectionVector2().x + transform.position.x, animController.GetDirectionVector2().y + transform.position.y);
	}
}
