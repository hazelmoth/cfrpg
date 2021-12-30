using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerInteractionRaycaster : MonoBehaviour 
{
	private const float RaycastDistance = 1f;
	private Actor player;
	private GameObject detected; // A cached gameobject detected in the current frame, if one exists.

	private void Start()
	{
		PlayerController.OnPlayerIdSet += GetPlayer;
	}

	private void LateUpdate()
	{
		detected = null;
	}

	/// Returns the gameobject on front of this player, if it implements IInteractable,
    /// IContinuouslyInteractable, or ISecondaryInteractable.
	public GameObject DetectInteractableObject () 
	{
		if (detected != null) return detected;
		if (player == null) GetPlayer();

		if (player == null)
		{
			Debug.LogError("Player wasn't found in scene??");
			return null;
		}

		Vector2 direction = player.Direction.ToVector2();
		RaycastHit2D hit = Physics2D.Raycast(player.transform.position, direction, RaycastDistance, ~ (1 << 8)); // Masks out layer 8, the Player layer.

		Debug.DrawRay (player.transform.position, direction * RaycastDistance, Color.green, Time.deltaTime, false);

		if (hit.collider != null)
		{
			GameObject entity = hit.collider.gameObject;

			// Detect entities which are covered by tilemap colliders (e.g., construction sites)
			if (hit.collider.GetComponent<TilemapCollider2D>())
			{
				Vector2 hitPos = hit.point + (direction * 0.5f); // Get a position inside the tile that was hit
				Vector2Int localPos = TilemapInterface.WorldPosToScenePos(hitPos, player.CurrentScene).ToVector2Int();
				
				entity = RegionMapManager.GetEntityObjectAtPoint(localPos, player.CurrentScene); // Find out what entity is on the tile in question.
			}

			// If it has an interactable component, return the entity.
			if (entity != null
                && (entity.TryGetComponent(out IInteractable _)
                || entity.TryGetComponent(out ISecondaryInteractable _)
                || entity.TryGetComponent(out IContinuouslyInteractable _)))
			{
				detected = entity;
				return detected;
			}
		}
		return null;
	}

	private void GetPlayer()
	{
		player = PlayerController.GetPlayerActor();
	}
}
