using Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Literally just loads the sprites for the actor
public class HumanSpriteLoader : MonoBehaviour
{
	public void LoadSprites(string raceId, string hairId, string hatId, string shirtId, string pantsId)
	{
		Sprite[] bodySprites = new Sprite[0];
		Sprite[] swooshSprites = new Sprite[0];
		Sprite[] hairSprites = new Sprite[4];
		Sprite[] hatSprites = new Sprite[4];
		Sprite[] shirtSprites = new Sprite[12];
		Sprite[] pantsSprites = new Sprite[12];

		ActorRace race = ContentLibrary.Instance.Races.GetById(raceId);
		if (race != null)
		{
			bodySprites = ContentLibrary.Instance.Races.GetById(raceId).BodySprites.ToArray();
			swooshSprites = ContentLibrary.Instance.Races.GetById(raceId).SwooshSprites.ToArray();
		}
		else {
			Debug.LogWarning("No race found for race ID " + raceId);
		}

		if (hairId != null)
		{
			Hair hair = ContentLibrary.Instance.Hairs.GetById(hairId);
			if (hair != null)
			{
				hairSprites = hair.sprites;
			}
		}
		if (hatId != null)
		{
			// TODO check that this cast is safe
			IHat hat = (IHat)ContentLibrary.Instance.Items.Get(hatId);
			if (hat != null)
				hatSprites = hat.GetHatSprites();
		}
		if (shirtId != null)
		{
			Shirt shirt = (Shirt)ContentLibrary.Instance.Items.Get(shirtId);
			if (shirt != null)
				shirtSprites = shirt.GetShirtSprites();
		}
		if (pantsId != null)
		{
			Pants pants = (Pants)ContentLibrary.Instance.Items.Get(pantsId);
			if (pants != null)
				pantsSprites = pants.GetPantsSprites();
		}

		this.GetComponent<ActorSpriteController>().SetSpriteArrays(bodySprites, swooshSprites, hairSprites, hatSprites, shirtSprites, pantsSprites);
	}
}
