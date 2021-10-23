using System;
using ContinentMaps;
using JetBrains.Annotations;
using UnityEngine;

/// A systemic MonoBehaviour which keeps track of which actor is the player,
/// and takes key inputs to control the player's movement.
public class PlayerController : MonoBehaviour
{
	/// Called after the ID of the player is set (there may not be a player actor
	/// at this point).
	public static event Action OnPlayerIdSet;
	
	/// Called after the camera rig has been set up for the player Actor.
	public static event Action OnPlayerObjectSet;

	[SerializeField] private GameObject cameraRigPrefab;

	private const float DiagonalSpeedMult = 1.2f; // How much faster diagonal movement is than 4-way

	private static Actor actor;
	private static ActorMovementController movement;
	private static bool hasSetupActor = false;
	private static GameObject cameraRig;
	private static string lastPlayerId; // The player ID for the previous frame
	
	/// The ID of the currently-controlled actor.
	public static string PlayerActorId { get; private set; }

	[UsedImplicitly]
	private void Update()
	{
		if (PauseManager.Paused) { return; }
		
		// Reset actor if player has changed
		if (PlayerActorId != lastPlayerId || actor == null) { hasSetupActor = false; }
		
		// Do nothing if there's no player to control.
		if (PlayerActorId == null)
		{
			hasSetupActor = false;
			return;
		}
		lastPlayerId = PlayerActorId;

		// Initialize for player actor
		if (!hasSetupActor)
		{
			Debug.Log("Setting up camera");
			SetupPlayerActor(cameraRigPrefab);
			hasSetupActor = true;
		}

		// MOVEMENT ---------------------------------------------------------------------------------------------------
		
		float horizontal = Input.GetAxisRaw("Horizontal");
		float vertical = Input.GetAxisRaw("Vertical");
		Vector2 movementVector = new Vector2(horizontal, vertical);

		// Equals 1 on an exact diagonal and 0 on manhattan movement
		float diagonalness = Math.Abs(movementVector.x) * Math.Abs(movementVector.y);
		float maxMagnitude = Mathf.Lerp(1f, DiagonalSpeedMult, diagonalness);

		if (movementVector.magnitude > maxMagnitude) {
			movementVector = movementVector.normalized * maxMagnitude;
		}

		// Ignore very small inputs
		if (Math.Abs(horizontal) < 0.05 && Math.Abs(vertical) < 0.05) movementVector = Vector2.zero;

		movement.SetWalking(movementVector);

		
		// HANDLE ATTACK INPUT ----------------------------------------------------------------------------------------
		
		if (Input.GetKeyDown(KeyCode.Space))
		{
			actor.GetComponent<ActorAttackHandler>().Attack();
		}
		
		
		// HANDLE WORLD BOUNDS DETECTION / REGION TRAVERSAL -----------------------------------------------------------

		if (actor.CurrentScene != SceneObjectManager.WorldSceneId) return;

		Vector2 playerPos = actor.Location.Vector2;
		// Detect if the player is at region edge
		if (!ContinentManager.CurrentRegion.info.disableAutoRegionTravel
			&& (playerPos.x < 0
				|| playerPos.y < 0
				|| playerPos.x > SaveInfo.RegionSize.x
				|| playerPos.y > SaveInfo.RegionSize.y))
		{
			// This should form a direction based on what side of the map the player's on
			Direction travelDirection = (playerPos - SaveInfo.RegionSize/2).ToDirection();
			Debug.Log(travelDirection);
			RegionTravel.TravelToAdjacent(actor, travelDirection, null, null);
		}
	}
	
	[UsedImplicitly]
	private void OnDestroy()
	{
		OnPlayerIdSet = null;
	}

	public static Actor GetPlayerActor() => actor;
	
	public static void SetPlayerActor(string actorId)
	{
		if (!ActorRegistry.IdIsRegistered(actorId))
		{
			Debug.LogError("Tried to set player-controlled actor with non-registered ID \"" + actorId + "\"!");
			return;
		}

		PlayerActorId = actorId;
		hasSetupActor = false;
		OnPlayerIdSet?.Invoke();
	}

	// Sets up the camera rig, and grabs the relevant components for the player actor.
	private static void SetupPlayerActor(GameObject cameraRigPrefab)
	{
		actor = ActorRegistry.Get(PlayerActorId).actorObject;
		if (actor == null)
		{
			Debug.LogError("Player actor doesn't have a gameobject!?");
		}
		movement = actor.GetComponent<ActorMovementController>();
		if (movement == null)
		{
			Debug.LogError("Player actor missing movement controller.");
		}
		if (!cameraRig)
		{
			cameraRig = Instantiate(cameraRigPrefab, actor.transform, false);
		}
		else
		{
			cameraRig.transform.SetParent(actor.transform, false);
		}
		OnPlayerObjectSet?.Invoke();
	}
}
