public class Player : Actor {

	public static Player instance;

	string hairId;

	void Awake () {
		instance = this;
	}
	// Use this for initialization
	void Start () {
		instance = this;
		actorCurrentScene = SceneObjectManager.GetSceneIdForObject(this.gameObject);
		inventory = GetComponent<ActorInventory> ();
		inventory.Initialize();
        LoadSprites();

		Inventory.OnHatEquipped += OnApparelEquipped;
		Inventory.OnPantsEquipped += OnApparelEquipped;
		Inventory.OnShirtEquipped += OnApparelEquipped;

		InventoryScreenManager.OnInventoryDrag += Inventory.AttemptMoveInventoryItem;
		InventoryScreenManager.OnInventoryDragOutOfWindow += Inventory.DropInventoryItem;
		PlayerInteractionManager.OnPlayerInteract += Inventory.OnInteractWithContainer;
	}

    void OnApparelEquipped (Item apparel)
    {
        LoadSprites();
    }
	// For when we need to set the instance before Start is called
	public static void SetInstance (Player instance)
	{
		Player.instance = instance;
	}
	// TODO move hair and body management to its own class
	public void SetHair(string hairId)
	{
		this.hairId = hairId;
	}
	// TODO SpriteUpdater class
    void LoadSprites()
    {
        string bodyId = "human_base";
        string hatId = null;
        string shirtId = null;
        string pantsId = null;
		if (inventory.GetEquippedHat() != null)
			hatId = inventory.GetEquippedHat().ItemId;
		if (inventory.GetEquippedShirt() != null)
			shirtId = inventory.GetEquippedShirt().ItemId;
		if (inventory.GetEquippedPants() != null)
			pantsId = inventory.GetEquippedPants().ItemId;

        GetComponent<HumanSpriteLoader>().LoadSprites(bodyId, hairId, hatId, shirtId, pantsId);
    }

}
