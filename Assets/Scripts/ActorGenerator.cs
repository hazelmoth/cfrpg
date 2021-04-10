using Items;
using System;
using System.Collections.Generic;
using System.Linq;
using ContentLibraries;
using UnityEngine;

public class ActorGenerator : MonoBehaviour
{
	private static System.Random random;
	
	/*
	 * Returns a new, completely random actor without any faction affiliation.
	 * Sets the actor ID to some ID which is currently unused, but does not
	 * register the actor with ActorRegistry.
	 */
    public static ActorData Generate ()
    {
        IList<Hair> hairPool = ContentLibrary.Instance.Hairs.GetAll().ToList();
        IList<ItemData> hatPool = ContentLibrary.Instance.Items.GetAll<IHat>();
        IList<ItemData> shirtPool = ContentLibrary.Instance.Items.GetAll<Shirt>();
        IList<ItemData> pantsPool = ContentLibrary.Instance.Items.GetAll<Pants>();
        IList<string> personalities = ContentLibrary.Instance.Personalities.GetAll();

        Hair hair = hairPool.PickRandom();
        ItemData hat = hatPool.PickRandom();
        ItemData shirt = shirtPool.PickRandom();
        ItemData pants = pantsPool.PickRandom();
        string personality = personalities.PickRandom();
        string race = "human_light";

		// 50% chance of no hat 
		if (random == null)
			random = new System.Random();
        if (random.Next(2) == 0)
            hat = null;

        Gender gender = GenderHelper.RandomGender();
        string name = NameGenerator.Generate(gender);
		ActorInventory.InvContents inv = new ActorInventory.InvContents
		{
			equippedHat = hat != null ? new ItemStack(hat) : null,
			equippedShirt = shirt != null ? new ItemStack(shirt) : null,
			equippedPants = pants != null ? new ItemStack(pants) : null
		};

		string profession = Professions.GetRandomSettlerProfession();

        return new ActorData(ActorRegistry.GetUnusedId(name),
	        name,
	        personality,
	        race,
	        gender,
	        hair.hairId,
	        new ActorPhysicalCondition(),
	        inv,
			0,
	        new FactionStatus(null),
			profession);
    }

	public static ActorData Generate(ActorTemplate template)
	{
		string race = template.races.Count > 0 ? template.races.PickRandom() : null;
		string personality = template.personalities.Count > 0 ? template.personalities.PickRandom() : null;
		string hat = template.hats.Count > 0 ? template.hairs.PickRandom() : null;
		string shirt = template.shirts.Count > 0 ? template.shirts.PickRandom() : null;
		string pants = template.pants.Count > 0 ? template.pants.PickRandom() : null;
		string hair = template.hairs.Count > 0 ? template.hairs.PickRandom() : null;
		string profession = template.hairs.Count > 0 ? template.hairs.PickRandom() : null;

		ActorRace raceData = ContentLibrary.Instance.Races.GetById(race);
		if (race != null && !raceData.SupportsHair)
		{
			hair = null;
		}

		// chance of no hat 
		if (random == null)
			random = new System.Random();
		if (random.NextDouble() > template.hatChance)
			hat = null;

		Gender gender = Gender.Male;
		if (random.NextDouble() < template.femaleChance)
			gender = Gender.Female;

		ActorInventory.InvContents inv = new ActorInventory.InvContents();
		inv.equippedHat = hat != null ? new ItemStack(hat, 1) : null;
		inv.equippedShirt = shirt != null ? new ItemStack(shirt, 1) : null;
		inv.equippedPants = pants != null ? new ItemStack(pants, 1) : null;

		string name = NameGenerator.Generate(gender);
		string id = ActorRegistry.GetUnusedId(name);

		return new ActorData(
			id,
			name,
			personality,
			race,
			gender,
			hair,
			new ActorPhysicalCondition(),
			inv,
			0,
			new FactionStatus(null),
			profession);
	}
}
