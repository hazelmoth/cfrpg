using System;
using JetBrains.Annotations;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public delegate void PlayerControllerEvent();
	public static event PlayerControllerEvent OnPlayerIdSet;

	[SerializeField] private GameObject cameraRigPrefab;
	
	private const float diagonalSpeedMult = 1.2f; // How much faster diagonal movement is than 4-way
	
	private static Actor actor;
	private static ActorMovementController movement;
	private static bool hasSetUpCamera = false;
	private static GameObject cameraRig;
	
	public static string PlayerActorId { get; private set; }

	[UsedImplicitly]
	private void Update()
	{
		// Do nothing if there's no player being controlled, or the game is paused
		if (PlayerActorId == null || actor == null || PauseManager.GameIsPaused)
		{
			return;
		}

		if (!hasSetUpCamera)
		{
			Debug.Log("Setting up camera");
			if (!cameraRig)
			{
				cameraRig = Instantiate(cameraRigPrefab, actor.transform, false);
			}
			else
			{
				cameraRig.transform.SetParent(actor.transform, false);
			}

			hasSetUpCamera = true;
		}

		float horizontal = Input.GetAxisRaw("Horizontal");
		float vertical = Input.GetAxisRaw("Vertical");
		Vector2 movementVector = new Vector2(horizontal, vertical);

		// Equals 1 on an exact diagonal and 0 on manhattan movement
		float diagonalness = movementVector.x * movementVector.y;
		float maxMagnitude = Mathf.Lerp(1f, diagonalSpeedMult, diagonalness);

		if (movementVector.magnitude > maxMagnitude) {
			movementVector = movementVector.normalized * maxMagnitude;
		}

		if (Math.Abs(horizontal) > 0.01 || Math.Abs(vertical) > 0.01)
		{
			movement.SetWalking(movementVector);
		}
		else
		{
			// Ignore very small movement inputs
			movement.SetWalking(Vector2.zero);
		}

		if (Input.GetKeyDown(KeyCode.Space))
		{
			actor.GetComponent<ActorAttackHandler>().Attack();
		}
	}
	[UsedImplicitly]
	private void OnDestroy()
	{
		OnPlayerIdSet = null;
	}

	public static void SetPlayerActor(string actorId)
	{
		if (!ActorRegistry.IdIsRegistered(actorId))
		{
			Debug.LogError("Tried to set player-controlled actor with non-registered ID \"" + actorId + "\"!");
			return;
		}
		actor = ActorRegistry.Get(actorId).actorObject;
		if (actor == null)
		{
			Debug.LogError("Player actor doesn't have a gameobject!?");
		}
		movement = actor.GetComponent<ActorMovementController>();
		if (movement == null)
		{
			Debug.LogError("Player actor missing movement controller.");
		}

		PlayerActorId = actorId;
		hasSetUpCamera = false;
		OnPlayerIdSet?.Invoke();
	}
}
