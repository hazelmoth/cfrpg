public class Player : Actor {

	public static Player instance;

	string hairId;
	bool hasInited = false;

	void Awake () {
		instance = this;
	}
	// Use this for initialization
	public void Init () {
		instance = this;
		actorCurrentScene = SceneObjectManager.GetSceneIdForObject(this.gameObject);
		inventory = new ActorInventory();
		inventory.Initialize();
        LoadSprites();

		Inventory.OnHatEquipped += OnApparelEquipped;
		Inventory.OnPantsEquipped += OnApparelEquipped;
		Inventory.OnShirtEquipped += OnApparelEquipped;

		InventoryScreenManager.OnInventoryDrag += Inventory.AttemptMoveInventoryItem;
		InventoryScreenManager.OnInventoryDragOutOfWindow += OnActivateItemDrop;
		PlayerInteractionManager.OnPlayerInteract += Inventory.OnInteractWithContainer;

		hasInited = true;
	}

    void OnApparelEquipped (Item apparel)
    {
        LoadSprites();
    }
	void OnActivateItemDrop (int slot, InventorySlotType type)
	{
		Inventory.DropInventoryItem(slot, type, TilemapInterface.WorldPosToScenePos(transform.position, CurrentScene), CurrentScene);
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
		LoadSprites();
	}
	public string GetHair()
	{
		return hairId;
	}
	// TODO SpriteUpdater class
    void LoadSprites()
    {
        string bodyId = "human_light";
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
