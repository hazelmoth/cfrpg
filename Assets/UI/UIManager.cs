using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

	// A script to manage cleanly switching between different menus (inventory, dialogue, etc.)
	// Apparently now it's also responsible for rescaling the inventory window when you open a container.
	// (Should the InventoryScreenManager be doing that? (probably.))

	public delegate void UiEvent ();
	public static event UiEvent OnOpenDialogueScreen;
	public static event UiEvent OnExitDialogueScreen;
	public static event UiEvent OnOpenInventoryScreen;
	public static event UiEvent OnExitInventoryScreen;
	public static event UiEvent OnOpenTaskAssignmentScreen;
	static UIManager instance;

	[SerializeField] GameObject invSelectedItemInfoPanel = null;
	[SerializeField] GameObject interactionTextCanvas = null;
	[SerializeField] GameObject inventoryScreenCanvas = null;
	[SerializeField] GameObject inventoryWindowPanel = null;
	[SerializeField] GameObject containerWindowPanel = null;
	[SerializeField] GameObject taskAssignmentCanvas = null;
	[SerializeField] GameObject notificationCanvas = null;
	[SerializeField] GameObject pauseMenuCanvas = null;
	[SerializeField] GameObject buildMenuCanvas = null;
	[SerializeField] GameObject dialogueCanvas = null;
	[SerializeField] GameObject hudCanvas = null;


	const float invWindowNormalWidth = 928.7f;
	const float invWindowNormalPosX = 0f;
	const float invWindowShortenedWidth = 750f;
	const float invWindowShortenedPosX = -299.1f;

	// Defines the pixel height for the container window given a number of inventory slot rows
	IDictionary<int, float> containerWindowHeightDict = new Dictionary<int, float> 
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

	// Clear event subscriptions when this object is destroyed 
	// (likely because the scene was unloaded)
	void ResetEventSubscriptions ()
	{
		OnOpenDialogueScreen = null;
		OnExitDialogueScreen = null;
		OnOpenInventoryScreen = null;
		OnExitInventoryScreen = null;
		OnOpenTaskAssignmentScreen = null; 
	}

	// Use this for initialization
	void Start () {
		instance = this;
		// Set all the canvases active in case they're disabled in the editor
		inventoryWindowPanel.SetActive (true);
		hudCanvas.SetActive (true);
		dialogueCanvas.SetActive (true);
		pauseMenuCanvas.SetActive (true);
		buildMenuCanvas.SetActive (true);
		notificationCanvas.SetActive (true);
		interactionTextCanvas.SetActive (true);

		BuildMenuManager.PopulateEntityMenu ();

		SwitchToMainHud ();
		SetInventoryWindowShortened (false);
		PlayerInteractionManager.OnPlayerInteract += OnPlayerInteract;
		PlayerInteractionManager.OnPlayerInitiateTaskGiving += OnInitiateTaskGiving;
		DialogueManager.OnInitiateDialogue += OnInitiateDialogue;
		DialogueManager.OnExitDialogue += OnExitDialogue;
        KeyInputHandler.OnBuildMenuButton += OnBuildMenuButton;
		BuildMenuManager.OnConstructButton += OnBuildMenuItemSelected;
		TaskAssignmentUIManager.OnExitTaskUI += SwitchToMainHud;

		SceneChangeManager.OnSceneExit += ResetEventSubscriptions;
    }
		


    // Update is called once per frame
    void Update () {
		if (Input.GetKeyDown(KeyCode.Tab) && !PauseManager.GameIsPaused) {
			if (inventoryScreenCanvas.activeInHierarchy) {
				// If both the dialogue and inventory screens are active when tab is pressed, switch to the dialogue screen
				if (dialogueCanvas.activeInHierarchy)
					SwitchToDialogueScreen ();
				else
					SwitchToMainHud ();
			} else {
				SwitchToInventoryScreen ();
			}
		}
	}
	void OnPlayerInteract (InteractableObject thing) {
		InteractableContainer container = thing as InteractableContainer;
		if (container != null && inventoryScreenCanvas.activeInHierarchy == false) {
			SwitchToContainerInventoryScreen ();
			ResizeContainerWindow (container.NumSlots);
		}
	}
	// From event in PlayerInteractionManager
	void OnInitiateTaskGiving (NPC npc)
	{
		SwitchToTaskAssignmentScreen();
	}
	// Called from OnInitiateDialogue event in DialogueManager
	void OnInitiateDialogue (NPC npc, DialogueDataMaster.DialogueNode startNode) {
		SwitchToDialogueScreen ();
		OnOpenDialogueScreen?.Invoke();
	}
	void OnExitDialogue () {
		SwitchToMainHud ();
		OnExitDialogueScreen?.Invoke();
	}
    void OnBuildMenuButton ()
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
	void OnBuildMenuItemSelected () {
		SwitchToMainHud ();
	}
    void SwitchToInventoryScreen () {
		SwitchToMainHud();
		inventoryScreenCanvas.SetActive (true);
		SetInventoryWindowShortened (false);
		OnOpenInventoryScreen?.Invoke();
	}
	void SwitchToContainerInventoryScreen () {
		SwitchToMainHud();
		inventoryScreenCanvas.SetActive (true);
		containerWindowPanel.SetActive (true);
		SetInventoryWindowShortened (true);
		OnOpenInventoryScreen?.Invoke();
	}
	void SwitchToDialogueScreen () {
		SwitchToMainHud();
		hudCanvas.SetActive(false);
		dialogueCanvas.SetActive (true);
	}
    void SwitchToBuildMenu ()
    {
		SwitchToMainHud();
        buildMenuCanvas.SetActive(true);
    }
	void SwitchToTaskAssignmentScreen ()
	{
		SwitchToMainHud();
		taskAssignmentCanvas.SetActive(true);
		OnOpenTaskAssignmentScreen?.Invoke();
	}
    void SwitchToMainHud () {
		if (inventoryScreenCanvas.activeInHierarchy && OnExitInventoryScreen != null) {
			OnExitInventoryScreen ();
		}
		inventoryScreenCanvas.SetActive (false);
		containerWindowPanel.SetActive (false);
		hudCanvas.SetActive (true);
		dialogueCanvas.SetActive (false);
        buildMenuCanvas.SetActive(false);
		taskAssignmentCanvas.SetActive(false);
	}
	void SetInventoryWindowShortened (bool shorten) {
		RectTransform windowRect = inventoryWindowPanel.GetComponent<RectTransform> ();
		if (shorten) {
			windowRect.localPosition = new Vector3 (invWindowShortenedPosX, windowRect.localPosition.y);
			windowRect.sizeDelta = new Vector2 (invWindowShortenedWidth, windowRect.sizeDelta.y);
			invSelectedItemInfoPanel.SetActive (false);
		} else {
			windowRect.localPosition = new Vector3 (invWindowNormalPosX, windowRect.localPosition.y);;
			windowRect.sizeDelta = new Vector2 (invWindowNormalWidth, windowRect.sizeDelta.y);
			invSelectedItemInfoPanel.SetActive (true);
		}
	}
	void ResizeContainerWindow (int slots) {
		int rows = Mathf.CeilToInt((float)slots / 6f);
		float height = containerWindowHeightDict[rows];
		RectTransform rect = containerWindowPanel.GetComponent<RectTransform> ();
		rect.sizeDelta = new Vector3 (rect.sizeDelta.x, height);
		rect.localPosition = new Vector3 (rect.localPosition.x, 0);
	}
}
