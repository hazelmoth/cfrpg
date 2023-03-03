using System.Collections.Generic;
using ActorComponents;
using ContinentMaps;
using Dialogue;
using GUI.MapView;
using UnityEngine;

namespace GUI
{
	// A script to manage cleanly switching between different menus (inventory, dialogue, etc.)
	// Apparently now it's also responsible for rescaling the inventory window when you open a container.
	// (Should the InventoryScreenManager be doing that? (probably.))
	public class UIManager : MonoBehaviour
	{
		public delegate void UiEvent();
		public static event UiEvent OnOpenDialogueScreen;
		public static event UiEvent OnExitDialogueScreen;
		public static event UiEvent OnOpenInventoryScreen;
		public static event UiEvent OnExitInventoryScreen;
		public static event UiEvent OnOpenSurvivorMenu;
		private static UIManager instance;

		[SerializeField] private GameObject invSelectedItemInfoPanel = null;
		[SerializeField] private GameObject interactionTextCanvas = null;
		[SerializeField] private GameObject inventoryScreenCanvas = null;
		[SerializeField] private GameObject inventoryWindowPanel = null;
		[SerializeField] private GameObject containerWindowPanel = null;
		[SerializeField] private GameObject taskAssignmentCanvas = null;
		[SerializeField] private GameObject notificationCanvas = null;
		[SerializeField] private GameObject timeDisplayCanvas = null;
		[SerializeField] private GameObject tradeMenuCanvas = null;
		[SerializeField] private GameObject pauseMenuCanvas = null;
		[SerializeField] private GameObject craftMenuCanvas = null;
		[SerializeField] private GameObject buildMenuCanvas = null;
		[SerializeField] private GameObject healthbarCanvas = null;
		[SerializeField] private GameObject cookMenuCanvas = null;
		[SerializeField] private GameObject dialogueCanvas = null;
		[SerializeField] private GameObject documentCanvas = null;
		[SerializeField] private GameObject hotbarCanvas = null;
		[SerializeField] private MapViewManager mapView = null;


		private const float InvWindowNormalWidth = 928.7f;
		private const float InvWindowNormalPosX = 0f;
		private const float InvWindowShortenedWidth = 750f;
		private const float InvWindowShortenedPosX = -299.1f;

		// Defines the pixel height for the container window given a number of inventory slot rows
		private readonly IDictionary<int, float> containerWindowHeightDict = new Dictionary<int, float>
		{
			{ 1, 480.8f },
			{ 2, 480.8f },
			{ 3, 480.8f },
			{ 4, 480.8f },
			{ 5, 552.9f },
			{ 6, 625.0f },
			{ 7, 699.1f },
			{ 8, 776.5f }
		};

		// The player actor ID, as of the previous frame
		private string currentPlayerId = null;

		private bool hasTimeDisplay;
		private bool hasHotbar;
		private bool hasHealthbar;

		// Use this for initialization
		private void Start()
		{
			instance = this;
			hasHealthbar = healthbarCanvas != null;
			hasHotbar = hotbarCanvas != null;
			hasTimeDisplay = timeDisplayCanvas != null;

			// Set all the canvases active in case they're disabled in the editor
			SetHudActive(true);
			inventoryWindowPanel.SetActive(true);
			documentCanvas.SetActive(true);
			cookMenuCanvas.SetActive(true);
			buildMenuCanvas.SetActive(true);
			craftMenuCanvas.SetActive(true);
			dialogueCanvas.SetActive(true);
			tradeMenuCanvas.SetActive(true);
			pauseMenuCanvas.SetActive(true);
			buildMenuCanvas.SetActive(true);
			notificationCanvas.SetActive(true);
			interactionTextCanvas.SetActive(true);
			mapView.SetVisible(false);

			BuildMenuManager.PopulateEntityMenu();

			SwitchToMainHud();
			SetInventoryWindowShortened(false);

			PlayerInteractionManager.OnPlayerInteract += OnPlayerInteract;
			PlayerInteractionManager.OnInteractWithSettler += OnInitiateTaskGiving;
			PlayerInteractionManager.OnTradeWithTrader += OnPlayerTrade;
			DialogueManager.OnInitiateDialogue += OnInitiateDialogue;
			DialogueManager.OnExitDialogue += OnExitDialogue;
			KeyInputHandler.OnBuildMenuButton += OnBuildMenuButton;
			BuildMenuManager.OnConstructButton += OnBuildMenuItemSelected;
			PlayerController.OnPlayerIdSet += OnPlayerIdSet;

			SceneChangeActivator.OnSceneExit += ResetStatics;
		}

		public static void CloseAllMenus()
		{
			instance.SwitchToMainHud();
		}

		public static void SwitchToDocumentCanvas()
		{
			instance.SwitchToMainHud();
			instance.documentCanvas.SetActive(true);
		}

		// Update is called once per frame
		private void Update()
		{
			// UI is frozen while game is paused
			if (PauseManager.Paused) return;
			
			// No HUD if no player actor
			if (PlayerController.PlayerActorId == null)
			{
				SwitchToMainHud();
				instance.SetHudActive(false);
				return;
			}

			if (currentPlayerId != PlayerController.PlayerActorId)
            {
				if (currentPlayerId != null && ActorRegistry.IdIsRegistered(currentPlayerId))
				{
					ActorInventory oldPlayerInv =
						ActorRegistry.Get(currentPlayerId).data.Get<ActorInventory>();
					if (oldPlayerInv != null) oldPlayerInv.OnContainerOpened -= HandlePlayerOpenedContainer;
				}

				currentPlayerId = PlayerController.PlayerActorId;
				ActorInventory playerInv = ActorRegistry.Get(currentPlayerId).data.Get<ActorInventory>();
				if (playerInv != null) playerInv.OnContainerOpened += HandlePlayerOpenedContainer;
				SwitchToMainHud();
            }
			
			if (Input.GetKeyDown(KeyCode.Tab))
			{
				if (DialogueManager.IsInDialogue) DialogueManager.ExitDialogue();

				if (inventoryScreenCanvas.activeInHierarchy) SwitchToMainHud();
				else SwitchToInventoryScreen();
			}
			else if (Input.GetKeyDown(KeyCode.C))
			{
				if (DialogueManager.IsInDialogue) DialogueManager.ExitDialogue();
				SwitchToCraftingMenu();
			}
			else if (Input.GetKeyDown(KeyCode.M))
			{
				if (DialogueManager.IsInDialogue) DialogueManager.ExitDialogue();
				if (mapView.CurrentlyVisible) {SwitchToMainHud();}
				else SwitchToMapView();
			}
		}

		private bool TryGetPlayerComp<T>(out T component) where T : IActorComponent
		{
			ActorRegistry.ActorInfo player = ActorRegistry.Get(PlayerController.PlayerActorId);
			if (player == null) { component = default; return false; }
			
			component = player.data.Get<T>();
			return component != null;
		}

		private void OnPlayerIdSet()
		{
			if (TryGetPlayerComp(out ActorInventory playerInv))
				playerInv.OnActiveContainerDestroyedOrNull += OnActiveContainerDestroyedOrNull;
		}

		// Clear event subscriptions when this object is destroyed 
		// (likely because the scene was unloaded)
		private void ResetStatics()
		{
			instance = null;
			OnOpenDialogueScreen = null;
			OnExitDialogueScreen = null;
			OnOpenInventoryScreen = null;
			OnExitInventoryScreen = null;
			OnOpenSurvivorMenu = null;
		}

		private void OnPlayerInteract(IInteractable thing)
		{
            if (thing is IContainer container && inventoryScreenCanvas.activeInHierarchy == false)
	            HandlePlayerOpenedContainer(container);

            if (thing is ICraftingStation) SwitchToCookMenu();
		}

        private void HandlePlayerOpenedContainer(IContainer container)
        {
	        if (!TryGetPlayerComp<ActorInventory>(out _)) return;
	        
			SwitchToContainerInventoryScreen();
			ResizeContainerWindow(container.SlotCount);
		}

		// From event in PlayerInteractionManager
		private void OnInitiateTaskGiving(Actor Actor)
		{
			SwitchToTaskAssignmentScreen();
		}
		
		// Called from OnInitiateDialogue event in DialogueManager
		private void OnInitiateDialogue(Actor actor)
		{
			SwitchToDialogueScreen();
			OnOpenDialogueScreen?.Invoke();
		}

		private void OnExitDialogue()
		{
			SwitchToMainHud();
			OnExitDialogueScreen?.Invoke();
		}

		private void OnBuildMenuButton()
		{
			if (PauseManager.Paused) return;
			if (!EntityConstructionManager.BuildingIsAllowed) return;
			if (buildMenuCanvas.activeInHierarchy) SwitchToMainHud();
			else SwitchToBuildMenu();
		}

		private void OnPlayerTrade(Actor actor, IContainer items, IWallet vendorWallet) => SwitchToTradeMenu();

		private void OnBuildMenuItemSelected() => SwitchToMainHud();

		private void OnActiveContainerDestroyedOrNull()
		{
			SetInventoryWindowShortened(false);
			containerWindowPanel.SetActive(false);
		}

		/// Only activate whichever elements of the HUD are relevant to the player actor
		private void SetHudActive(bool active)
		{
			if (hasTimeDisplay) timeDisplayCanvas.SetActive(active);
			if (hasHotbar)
				hotbarCanvas.SetActive(active && TryGetPlayerComp<ActorInventory>(out _));
			if (hasHealthbar)
				healthbarCanvas.SetActive(active && TryGetPlayerComp<ActorHealth>(out _));
		}

		private void SwitchToMainHud()
		{
			if (inventoryScreenCanvas.activeInHierarchy && OnExitInventoryScreen != null)
			{
				OnExitInventoryScreen();
			}

			SetHudActive(true);
			inventoryScreenCanvas.SetActive(false);
			containerWindowPanel.SetActive(false);
			taskAssignmentCanvas.SetActive(false);
			dialogueCanvas.SetActive(false);
			craftMenuCanvas.SetActive(false);
			cookMenuCanvas.SetActive(false);
			buildMenuCanvas.SetActive(false);
			tradeMenuCanvas.SetActive(false);
			documentCanvas.SetActive(false);
			mapView.SetVisible(false);
		}

		private void SwitchToInventoryScreen()
		{
			SwitchToMainHud();
			inventoryScreenCanvas.SetActive(true);
			SetInventoryWindowShortened(false);
			OnOpenInventoryScreen?.Invoke();
		}

		private void SwitchToContainerInventoryScreen()
		{
			SwitchToMainHud();
			inventoryScreenCanvas.SetActive(true);
			containerWindowPanel.SetActive(true);
			SetInventoryWindowShortened(true);
			OnOpenInventoryScreen?.Invoke();
		}

		private void SwitchToDialogueScreen()
		{
			SwitchToMainHud();
			SetHudActive(false);
			dialogueCanvas.SetActive(true);
		}

		private void SwitchToCraftingMenu()
		{
			SwitchToMainHud();
			craftMenuCanvas.SetActive(true);
		}

		private void SwitchToBuildMenu()
		{
			SwitchToMainHud();
			buildMenuCanvas.SetActive(true);
		}

		private void SwitchToCookMenu()
		{
			SwitchToMainHud();
			cookMenuCanvas.SetActive(true);
		}

		private void SwitchToTradeMenu()
		{
			SwitchToMainHud();
			tradeMenuCanvas.SetActive(true);
		}

		private void SwitchToTaskAssignmentScreen()
		{
			SwitchToMainHud();
			taskAssignmentCanvas.SetActive(true);
			OnOpenSurvivorMenu?.Invoke();
		}

		private void SwitchToMapView()
		{
			SwitchToMainHud();
			SetHudActive(false);
			mapView.SetVisible(true);
			// mapView.RenderMap(ContinentManager.LoadedMap);
		}

		private void SetInventoryWindowShortened(bool shorten)
		{
			RectTransform windowRect = inventoryWindowPanel.GetComponent<RectTransform>();
			if (shorten)
			{
				windowRect.localPosition = new Vector3(InvWindowShortenedPosX, windowRect.localPosition.y);
				windowRect.sizeDelta = new Vector2(InvWindowShortenedWidth, windowRect.sizeDelta.y);
				invSelectedItemInfoPanel.SetActive(false);
			}
			else
			{
				windowRect.localPosition = new Vector3(InvWindowNormalPosX, windowRect.localPosition.y); ;
				windowRect.sizeDelta = new Vector2(InvWindowNormalWidth, windowRect.sizeDelta.y);
				invSelectedItemInfoPanel.SetActive(true);
			}
		}

		private void ResizeContainerWindow(int slots)
		{
			int rows = Mathf.CeilToInt(slots / 6f);
			float height = containerWindowHeightDict[rows];
			RectTransform rect = containerWindowPanel.GetComponent<RectTransform>();
			rect.sizeDelta = new Vector3(rect.sizeDelta.x, height);
			rect.localPosition = new Vector3(rect.localPosition.x, 0);
		}
	}
}
