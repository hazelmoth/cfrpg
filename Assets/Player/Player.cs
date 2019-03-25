using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Actor {

	public static Player instance;

	// Use this for initialization
	void Start () {
		instance = this;
		actorCurrentScene = this.gameObject.scene.name;
        GetComponent<PlayerInventory>().Initialize();
        LoadSprites();

        PlayerInventory.OnHatEquipped += OnApparelEquipped;
        PlayerInventory.OnPantsEquipped += OnApparelEquipped;
        PlayerInventory.OnShirtEquipped += OnApparelEquipped;
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
        if (PlayerInventory.GetEquippedHat() != null)
            hatId = PlayerInventory.GetEquippedHat().itemId;
        if (PlayerInventory.GetEquippedShirt() != null)
            shirtId = PlayerInventory.GetEquippedShirt().itemId;
        if (PlayerInventory.GetEquippedPants() != null)
            pantsId = PlayerInventory.GetEquippedPants().itemId;

        GetComponent<HumanSpriteLoader>().LoadSprites(bodyId, hairId, hatId, shirtId, pantsId);
    }

}
