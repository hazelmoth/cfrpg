using ActorComponents;
using Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ActorGenerator : MonoBehaviour
{
	private static System.Random random;
    public static ActorData Generate ()
    {
        IList<Hair> hairPool = ContentLibrary.Instance.Hairs.GetHairs();
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

        string hatId = hat.ItemId;

		// 50% chance of no hat 
		if (random == null)
			random = new System.Random();
        if (random.Next(2) == 0)
            hatId = null;


        Gender gender = GenderHelper.RandomGender();
        string name = NameGenerator.Generate(gender);
		ActorInventory.InvContents inv = new ActorInventory.InvContents();
		
		inv.equippedHat = hat != null ? new Item(hat) : null;
		inv.equippedShirt = shirt != null ? new Item(shirt) : null;
		inv.equippedPants = pants != null ? new Item(pants) : null;

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
			new List<object>());
    }

	public static ActorData Generate(CharacterGenTemplate template)
	{
		string hair = template.hairs.Count > 0 ? template.hairs.PickRandom() : null;
		string hat = template.hats.Count > 0 ? template.hairs.PickRandom() : null;
		string shirt = template.shirts.Count > 0 ? template.shirts.PickRandom() : null;
		string pants = template.pants.Count > 0 ? template.pants.PickRandom() : null;
		string personality = template.personalities.Count > 0 ? template.personalities.PickRandom() : null;
		string race = template.races.Count > 0 ? template.races.PickRandom() : null;


		// chance of no hat 
		if (random == null)
			random = new System.Random();
		if (random.NextDouble() > template.hatChance)
			hat = null;

		Gender gender = Gender.Male;
		if (random.NextDouble() < template.femaleChance)
			gender = Gender.Female;
			
		string name = NameGenerator.Generate(gender);
		ActorInventory.InvContents inv = new ActorInventory.InvContents();

		inv.equippedHat = hat != null ? new Item(hat, 1) : null;
		inv.equippedShirt = shirt != null ? new Item(shirt, 1) : null;
		inv.equippedPants = pants != null ? new Item(pants, 1) : null;
		string id = ActorRegistry.GetUnusedId(name);

		List<object> components = new List<object>();
		for (int i = 0; i < template.components.Count; i++)
		{
			string typeName = "ActorComponents." + template.components[i];
			Type type = System.Type.GetType(template.components[i]);
			if (type == null)
			{
				Debug.LogError("Actor component of type \"" + typeName + "\" not found");
				continue;
			}
			object instantiated = type.GetConstructor(new Type[] { typeof(string) }).Invoke(new object[] { id });
			components.Add(instantiated);
		}

		return new ActorData(ActorRegistry.GetUnusedId(name),
			name,
			personality,
			race,
			gender,
			hair,
			new ActorPhysicalCondition(),
			inv,
			0,
			new FactionStatus(null),
			new List<object>());
	}

	// Returns a newly generated actor of the given race without any clothing or hair
	public static ActorData GenerateAnimal(string race)
    {
	    IList<string> personalities = ContentLibrary.Instance.Personalities.GetAll();

	    string personality = personalities.PickRandom();

	    Gender gender = GenderHelper.RandomGender();
	    string name = NameGenerator.Generate(gender);
	    ActorInventory.InvContents inv = new ActorInventory.InvContents();
		string id = ActorRegistry.GetUnusedId(name);

		return new ActorData(
			id,
		    name,
		    personality,
		    race,
		    gender,
		    null,
		    new ActorPhysicalCondition(),
		    inv,
			0,
		    new FactionStatus(null),
			new List<object>
			{
				new Trader(id)
			}
		);
    }
}
