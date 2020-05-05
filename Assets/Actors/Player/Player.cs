public class Player : Actor {

	public static Player instance;

	private string hairId;
	private bool hasInited = false;
	private ActorInventory inventory;

	private void Awake () {
		instance = this;
	}
	// Use this for initialization
	public void Init (string actorId) {
		instance = this;
		ActorId = actorId;
		//CurrentScene = SceneObjectManager.GetSceneIdForObject(this.gameObject);
		GetData().Inventory = new ActorInventory();
		inventory = GetData().Inventory;
		inventory.Initialize();
        LoadSprites();

        inventory.OnHatEquipped += OnApparelEquipped;
        inventory.OnPantsEquipped += OnApparelEquipped;
        inventory.OnShirtEquipped += OnApparelEquipped;

		PlayerInteractionManager.OnPlayerInteract += inventory.OnInteractWithContainer;

		hasInited = true;
	}

	private void OnApparelEquipped (Item apparel)
    {
        LoadSprites();
    }

	private void OnActivateItemDrop (int slot, InventorySlotType type)
	{
		inventory.DropInventoryItem(slot, type, TilemapInterface.WorldPosToScenePos(transform.position, CurrentScene), CurrentScene);
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
	private void LoadSprites()
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
