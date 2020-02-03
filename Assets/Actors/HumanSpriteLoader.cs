using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Literally just loads the sprites for the actor
public class HumanSpriteLoader : MonoBehaviour
{
	public void LoadSprites(string bodySpriteId, string hairId, string hatId, string shirtId, string pantsId)
	{
		Sprite[] bodySprites = Resources.LoadAll<Sprite>("Sprites/" + bodySpriteId);
		Sprite[] hairSprites = new Sprite[4];
		Sprite[] hatSprites = new Sprite[4];
		Sprite[] shirtSprites = new Sprite[12];
		Sprite[] pantsSprites = new Sprite[12];
		if (hairId != null)
		{
			Hair hair = HairLibrary.GetHairById(hairId);
			if (hair != null)
			{
				hairSprites = hair.sprites;
			}
		}
		if (hatId != null)
		{
			// TODO check that this cast is safe
			Hat hat = (Hat)ItemLibrary.GetItemById(hatId);
			if (hat != null)
				hatSprites = hat.GetHatSprites();
		}
		if (shirtId != null)
		{
			Shirt shirt = (Shirt)ItemLibrary.GetItemById(shirtId);
			if (shirt != null)
				shirtSprites = shirt.GetShirtSprites();
		}
		if (pantsId != null)
		{
			Pants pants = (Pants)ItemLibrary.GetItemById(pantsId);
			if (pants != null)
				pantsSprites = pants.GetPantsSprites();
		}

		this.GetComponent<HumanSpriteController>().SetSpriteArrays(bodySprites, hairSprites, hatSprites, shirtSprites, pantsSprites);
	}
}
