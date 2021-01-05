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
		if (hit.collider != null)
		{
			if (hit.collider.TryGetComponent(out IInteractableObject interactable))
				return hit.collider.gameObject;

			// Detect entities which are covered by tilemap colliders (e.g., construction sites)
			if (hit.collider.GetComponent<TilemapCollider2D>())
			{
				Vector2 hitPos = hit.point + (direction * 0.5f); // Get a position inside the tile that was hit
				Vector2Int localPos = TilemapInterface.WorldPosToScenePos(hitPos, player.CurrentScene).ToVector2Int();

				// Find out what entity is on the tile in question.
				GameObject entity = WorldMapManager.GetEntityObjectAtPoint(localPos, player.CurrentScene);
				// Check if it's an interactable.
				if (entity != null && entity.TryGetComponent(out IInteractableObject tiledInteractable))
				{
					return entity;
				}
			}
		}
		return null;
	}

	private void GetPlayer()
	{
		player = ActorRegistry.Get(PlayerController.PlayerActorId).actorObject;
	}
}
