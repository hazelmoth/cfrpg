using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Literally just loads the sprites for the NPC
public class NPCSpriteLoader : MonoBehaviour {

	public void LoadSprites (string bodySpriteId) {
		Sprite[] sprites = Resources.LoadAll<Sprite> ("Sprites/" + bodySpriteId);
		this.GetComponent<HumanSpriteController> ().SetBodySpriteArray (sprites);
	}
	public void LoadSprites (string bodySpriteId, string hatId, string shirtId, string pantsId) {
		Debug.Log (bodySpriteId);
		Sprite[] bodySprites = Resources.LoadAll<Sprite> ("Sprites/" + bodySpriteId);
        Sprite[] hairSprites = HairLibrary.GetHairs()[0].sprites; //TODO get by ID
		Sprite[] hatSprites = new Sprite[4];
		Sprite[] shirtSprites = new Sprite[12];
		Sprite[] pantsSprites = new Sprite[12];
		if (hatId != null) {
			Hat hat = (Hat)ItemManager.GetItemById (hatId);
			if (hat != null)
				hatSprites = hat.GetHatSprites ();
		}
		if (shirtId != null) {
			Shirt shirt = (Shirt)ItemManager.GetItemById (shirtId);
			if (shirt != null)
				shirtSprites = shirt.GetShirtSprites ();
		}
		if (pantsId != null) {
			Pants pants = (Pants)ItemManager.GetItemById (pantsId);
			if (pants != null)
				pantsSprites = pants.GetPantsSprites ();
		}

		this.GetComponent<HumanSpriteController> ().SetSpriteArrays (bodySprites, hairSprites, hatSprites, shirtSprites, pantsSprites);
	}
}
