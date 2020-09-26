using System.Collections.Generic;
using UnityEngine;

namespace GUI
{
	public class UIManager : MonoBehaviour
	{

		// A script to manage cleanly switching between different menus (inventory, dialogue, etc.)
		// Apparently now it's also responsible for rescaling the inventory window when you open a container.
		// (Should the InventoryScreenManager be doing that? (probably.))

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
		[SerializeField] private GameObject tradeMenuCanvas = null;
		[SerializeField] private GameObject pauseMenuCanvas = null;
		[SerializeField] private GameObject craftMenuCanvas = null;
		[SerializeField] private GameObject buildMenuCanvas = null;
		[SerializeField] private GameObject cookMenuCanvas = null;
		[SerializeField] private GameObject dialogueCanvas = null;
		[SerializeField] private GameObject hudCanvas = null;


		private const float invWindowNormalWidth = 928.7f;
		private const float invWindowNormalPosX = 0f;
		private const float invWindowShortenedWidth = 750f;
		private const float invWindowShortenedPosX = -299.1f;

		// Defines the pixel height for the container window given a number of inventory slot rows
		private IDictionary<int, float> containerWindowHeightDict = new Dictionary<int, float>
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


		// Use this for initialization
		private void Start()
		{
			instance = this;
			// Set all the canvases active in case they're disabled in the editor
			inventoryWindowPanel.SetActive(true);
			hudCanvas.SetActive(true);
			cookMenuCanvas.SetActive(true);
			buildMenuCanvas.SetActive(true);
			craftMenuCanvas.SetActive(true);
			dialogueCanvas.SetActive(true);
			tradeMenuCanvas.SetActive(true);
			pauseMenuCanvas.SetActive(true);
			buildMenuCanvas.SetActive(true);
			notificationCanvas.SetActive(true);
			interactionTextCanvas.SetActive(true);

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
			SurvivorMenuManager.OnExit += SwitchToMainHud;
			PlayerController.OnPlayerIdSet += OnPlayerIdSet;

			SceneChangeActivator.OnSceneExit += ResetStatics;
		}

		public static void CloseAllMenus()
		{
			instance.SwitchToMainHud();
		}


		// Update is called once per frame
		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Tab) && !PauseManager.GameIsPaused)
			{
				if (inventoryScreenCanvas.activeInHierarchy)
				{
					// If both the dialogue and inventory screens are active when tab is pressed, switch to the dialogue screen
					if (dialogueCanvas.activeInHierarchy)
						SwitchToDialogueScreen();
					else
						SwitchToMainHud();
				}
				else
				{
					SwitchToInventoryScreen();
				}
			}
			else if (Input.GetKeyDown(KeyCode.C) && !PauseManager.GameIsPaused)
			{
				SwitchToCraftingMenu();
			}
		}

		private void OnPlayerIdSet()
		{
			ActorRegistry.Get(PlayerController.PlayerActorId).data.Inventory.OnActiveContainerDestroyedOrNull += OnActiveContainerDestroyedOrNull;
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

		private void OnPlayerInteract(IInteractableObject thing)
		{
			InteractableContainer container = thing as InteractableContainer;
			if (container != null && inventoryScreenCanvas.activeInHierarchy == false)
			{
				SwitchToContainerInventoryScreen();
				ResizeContainerWindow(container.NumSlots);
			}
			if (thing is ICraftingStation craftingStation)
			{
				SwitchToCookMenu();
			}
		}
		// From event in PlayerInteractionManager
		private void OnInitiateTaskGiving(Actor Actor)
		{
			SwitchToTaskAssignmentScreen();
		}
		// Called from OnInitiateDialogue event in DialogueManager
		private void OnInitiateDialogue(Actor Actor, DialogueDataMaster.DialogueNode startNode)
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
			if (PauseManager.GameIsPaused)
				return;
			if (buildMenuCanvas.activeInHierarchy)
			{
				SwitchToMainHud();
			}
			else
			{
				SwitchToBuildMenu();
			}
		}

		private void OnPlayerTrade(Actor actor)
		{
			SwitchToTradeMenu();
		}

		private void OnBuildMenuItemSelected()
		{
			SwitchToMainHud();
		}

		private void OnActiveContainerDestroyedOrNull()
		{
			SetInventoryWindowShortened(false);
			containerWindowPanel.SetActive(false);
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
			hudCanvas.SetActive(false);
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

		private void SwitchToMainHud()
		{
			if (inventoryScreenCanvas.activeInHierarchy && OnExitInventoryScreen != null)
			{
				OnExitInventoryScreen();
			}
			hudCanvas.SetActive(true);
			inventoryScreenCanvas.SetActive(false);
			containerWindowPanel.SetActive(false);
			dialogueCanvas.SetActive(false);
			craftMenuCanvas.SetActive(false);
			cookMenuCanvas.SetActive(false);
			buildMenuCanvas.SetActive(false);
			tradeMenuCanvas.SetActive(false);
			taskAssignmentCanvas.SetActive(false);
		}

		private void SetInventoryWindowShortened(bool shorten)
		{
			RectTransform windowRect = inventoryWindowPanel.GetComponent<RectTransform>();
			if (shorten)
			{
				windowRect.localPosition = new Vector3(invWindowShortenedPosX, windowRect.localPosition.y);
				windowRect.sizeDelta = new Vector2(invWindowShortenedWidth, windowRect.sizeDelta.y);
				invSelectedItemInfoPanel.SetActive(false);
			}
			else
			{
				windowRect.localPosition = new Vector3(invWindowNormalPosX, windowRect.localPosition.y); ;
				windowRect.sizeDelta = new Vector2(invWindowNormalWidth, windowRect.sizeDelta.y);
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