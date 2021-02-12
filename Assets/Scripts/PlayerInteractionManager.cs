using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

// Accesses PlayerInteractionRaycaster to check whether an interactable object or dropped item is
// present and take keyboard input to activate an interaction. Interacting with an object triggers
// the appropriate response in UIManager, or whatever actions the item is meant to perform.
public class PlayerInteractionManager : MonoBehaviour
{

	public delegate void PlayerInteractionEvent(IInteractable activatedObject);
	public delegate void PlayerActorInteractionEvent(Actor Actor);
	public static event PlayerInteractionEvent OnPlayerInteract;
	public static event PlayerActorInteractionEvent OnInteractWithSettler;
	public static event PlayerActorInteractionEvent OnTradeWithTrader;
	private PlayerInteractionRaycaster raycaster;
	private PickupDetector itemDetector;

	private static bool InteractKeyDown => Input.GetKeyDown(KeyCode.E);
	private static bool InteractKeyHeld => Input.GetKey(KeyCode.E);

	private void OnDestroy()
	{
		OnPlayerInteract = null;
		OnInteractWithSettler = null;
		OnTradeWithTrader = null;
	}

	// Use this for initialization
	private void Start()
	{
		raycaster = FindObjectOfType<PlayerInteractionRaycaster>();
		SetComponents();
		PlayerController.OnPlayerIdSet += SetComponents;
	}

	// Update is called once per frame
	private void Update()
	{
		if (PauseManager.Paused)
		{
			return;
		}

		IPickuppable detectedItem = itemDetector.GetCurrentDetectedItem();
		GameObject detectedObject = raycaster.DetectInteractableObject();

		// Items take priority over entities
		if (detectedItem != null)
		{
			if (InteractKeyDown)
			{
				PickupSystem.AttemptPickup(ActorRegistry.Get(PlayerController.PlayerActorId).actorObject, detectedItem);
			}
		}
		else if (detectedObject != null)
		{
			if (InteractKeyDown)
			{
				IInteractable[] interactables = detectedObject.GetComponents<IInteractable>();
				foreach (IInteractable detectedInteractable in interactables)
				{
					OnPlayerInteract?.Invoke(detectedInteractable);
					detectedInteractable.OnInteract();

					IBed bed = detectedObject.GetComponent<IBed>();
					if (bed != null)
					{
						PlayerSleep.SleepToMorning();
					}

					// Message the player's inventory that there may be an active container.
					// Note that this does not support multiple container components on one entity.
					if (detectedInteractable is IContainer)
						ActorRegistry.Get(PlayerController.PlayerActorId).data.Inventory.OnInteractWithContainer(detectedInteractable);
				}
			}
			if (InteractKeyHeld)
			{
				IContinuouslyInteractable[] interactables = detectedObject.GetComponents<IContinuouslyInteractable>();
				foreach (IContinuouslyInteractable detectedInteractable in interactables)
				{
					// OnPlayerInteract?.Invoke(detectedInteractable);
					detectedInteractable.Interact();
				}
			}
			else if (Input.GetKeyDown(KeyCode.F))
			{
				Actor detectedActor = detectedObject.GetComponent<Actor>();
				if (detectedActor != null)
				{
					// Only allow task delegation if this Actor is in the player's settlement
					if (detectedActor.GetData().FactionStatus.FactionId != null && detectedActor.GetData().FactionStatus.FactionId == ActorRegistry.Get(PlayerController.PlayerActorId).data.FactionStatus.FactionId)
					{
						OnInteractWithSettler?.Invoke(detectedActor);
					}
					else if (detectedActor.GetData().Profession == Professions.TraderProfessionID)
					{
						Debug.Log("Trading with a trader.");
						OnTradeWithTrader?.Invoke(detectedActor);
					}
				}
			}
		}
	}


	private void SetComponents()
	{
		Actor playerObject = ActorRegistry.Get(PlayerController.PlayerActorId)?.actorObject;
		if (playerObject != null)
		{
			itemDetector = playerObject.GetComponent<PickupDetector>();
		}
		else
		{
			itemDetector = null;
		}
	}
}
