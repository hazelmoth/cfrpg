using System;
using ActorComponents;
using Items;
using Popcron.Console;
using UnityEngine;

/// Accesses PlayerInteractionRaycaster to check whether an interactable object or dropped item is
/// present and take keyboard input to activate an interaction. Interacting with an object triggers
/// the appropriate response in UIManager, or whatever actions the item is meant to perform.
public class PlayerInteractionManager : MonoBehaviour
{
	public delegate void PlayerInteractionEvent(IInteractable activatedObject);
	public delegate void PlayerActorInteractionEvent(Actor actor);
	public static event PlayerInteractionEvent OnPlayerInteract;
	public static event PlayerActorInteractionEvent OnInteractWithSettler;

	/// Triggered when the player initiates trading. Takes the actor being traded with
	/// and the container with that actor's items.
	public static event Action<Actor, IContainer, IWallet> OnTradeWithTrader;
	
	private PlayerInteractionRaycaster raycaster;
	private PickupDetector itemDetector;
	private ActorInventory inventory;
	private ActorWallet wallet;
	
	private static bool InteractKeyDown => Input.GetKeyDown(KeyCode.E);
	private static bool SecondaryInteractKeyDown => Input.GetKeyDown(KeyCode.R);
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
		PlayerController.OnPlayerObjectSet += SetComponents;
	}

	// Update is called once per frame
	private void Update()
	{
		if (PauseManager.Paused) return;
		if (PlayerController.GetPlayerActor() == null) return;

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
					if (detectedInteractable is IInteractableContainer container)
						PlayerController.GetPlayerActor().GetData().Get<ActorInventory>()?.OpenContainer(container);
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
		}
		if (Input.GetKeyDown(KeyCode.F))
		{
			if (detectedObject == null) return;
			if (detectedObject.TryGetComponent(out Actor detectedActor))
			{
				// Only allow task delegation if this Actor is in the player's settlement
				FactionStatus factionStatus = detectedActor.GetData().Get<FactionStatus>();
				FactionStatus playerFactionStatus =
					PlayerController.GetPlayerActor().GetData().Get<FactionStatus>();
				
				if (factionStatus?.FactionId != null
				    && factionStatus.FactionId == playerFactionStatus?.FactionId)
				{
					OnInteractWithSettler?.Invoke(detectedActor);
				}
				else if (detectedActor.GetData().RoleId == Roles.Trader) InitiateTrade(detectedActor.ActorId);
			}
		}
		if (SecondaryInteractKeyDown && detectedObject != null)
		{
			ISecondaryInteractable[] interactables = detectedObject.GetComponents<ISecondaryInteractable>();
			foreach (ISecondaryInteractable detectedInteractable in interactables)
			{
				OnPlayerInteract?.Invoke(detectedInteractable);
				detectedInteractable.OnSecondaryInteract(PlayerController.GetPlayerActor());
			}
		}
	}

	/// Initiates a trade with the specified actor. If the actor is at a store, uses the
	/// store's inventory as the container; otherwise, uses the actor's inventory.
	[Command("init_trade")]
	public static void InitiateTrade(string nonPlayerActorId)
	{
		ActorRegistry.ActorInfo actor = ActorRegistry.Get(nonPlayerActorId);
		ActorInventory actorInventory = actor.data.Get<ActorInventory>();
		ActorWallet actorWallet = actor.data.Get<ActorWallet>();
		IOccupiable nonPlayerWorkstation = actor.actorObject.CurrentWorkstation;
		ShopStation shopStation = nonPlayerWorkstation as ShopStation;
		
		IContainer inventory = shopStation != null
			? shopStation
			: actorInventory;
		IWallet wallet = shopStation != null
			? shopStation
			: actorWallet;

		if (inventory == null || wallet == null)
		{
			Debug.LogWarning("Actor missing inventory or wallet; can't trade");
			return;
		}
		
		OnTradeWithTrader?.Invoke(actor.actorObject, inventory, wallet);
	}


	private void SetComponents()
	{
		Actor playerObject = ActorRegistry.Get(PlayerController.PlayerActorId)?.actorObject;
		itemDetector = playerObject != null ? playerObject.GetComponent<PickupDetector>() : null;
		inventory = playerObject != null ? playerObject.GetData().Get<ActorInventory>() : null;
		wallet = playerObject != null ? playerObject.GetData().Get<ActorWallet>() : null;
	}
}
