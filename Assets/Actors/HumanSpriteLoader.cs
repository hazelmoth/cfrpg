using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Literally just loads the sprites for the actor
public class HumanSpriteLoader : MonoBehaviour
{
	public void LoadSprites(string raceId, string hairId, string hatId, string shirtId, string pantsId)
	{
		Sprite[] bodySprites = new Sprite[0];
		Sprite[] hairSprites = new Sprite[4];
		Sprite[] hatSprites = new Sprite[4];
		Sprite[] shirtSprites = new Sprite[12];
		Sprite[] pantsSprites = new Sprite[12];

		ActorRace race = ContentLibrary.Instance.Races.GetById(raceId);
		if (race != null)
		{
			bodySprites = ContentLibrary.Instance.Races.GetById(raceId).BodySprites.ToArray();
		}
		else {
			Debug.LogWarning("No race found for race ID " + raceId);
		}

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
			Hat hat = (Hat)ContentLibrary.Instance.Items.GetItemById(hatId);
			if (hat != null)
				hatSprites = hat.GetHatSprites();
		}
		if (shirtId != null)
		{
			Shirt shirt = (Shirt)ContentLibrary.Instance.Items.GetItemById(shirtId);
			if (shirt != null)
				shirtSprites = shirt.GetShirtSprites();
		}
		if (pantsId != null)
		{
			Pants pants = (Pants)ContentLibrary.Instance.Items.GetItemById(pantsId);
			if (pants != null)
				pantsSprites = pants.GetPantsSprites();
		}

		this.GetComponent<HumanSpriteController>().SetSpriteArrays(bodySprites, hairSprites, hatSprites, shirtSprites, pantsSprites);
	}
}
