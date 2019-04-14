using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Actor {

	public static Player instance;

	void Awake () {
		instance = this;
	}
	// Use this for initialization
	void Start () {
		instance = this;
		actorCurrentScene = SceneObjectManager.GetSceneIdForObject(this.gameObject);
		inventory = GetComponent<ActorInventory> ();
		Inventory.Initialize();
        LoadSprites();

		Inventory.OnHatEquipped += OnApparelEquipped;
		Inventory.OnPantsEquipped += OnApparelEquipped;
		Inventory.OnShirtEquipped += OnApparelEquipped;
		Debug.Log (instance);
	}

    void OnApparelEquipped (Item apparel)
    {
        LoadSprites();
    }
    void LoadSprites()
    {
        string bodyId = "human_base";
        string hairId = "hair_short_brown";
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
