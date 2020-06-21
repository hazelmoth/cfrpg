using Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorGenerator : MonoBehaviour
{
	private static System.Random random;
    public static ActorData Generate ()
    {
        IList<Hair> hairPool = ContentLibrary.Instance.Hairs.GetHairs();
        IList<ItemData> hatPool = ContentLibrary.Instance.Items.GetHats();
        IList<ItemData> shirtPool = ContentLibrary.Instance.Items.GetShirts();
        IList<ItemData> pantsPool = ContentLibrary.Instance.Items.GetPants();
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
		
		inv.equippedHat = new Item(hat);
		inv.equippedShirt = new Item(shirt);
		inv.equippedPants = new Item(pants);

        return new ActorData(ActorRegistry.GetUnusedId(name),
	        name,
	        personality,
	        race,
	        gender,
	        hair.hairId,
	        new ActorPhysicalCondition(),
	        inv,
	        new FactionStatus(null));
    }
    
    // Returns a newly generated actor of the given race without any clothing or hair
    public static ActorData GenerateAnimal(string race)
    {
	    IList<string> personalities = ContentLibrary.Instance.Personalities.GetAll();

	    string personality = personalities.PickRandom();

	    Gender gender = GenderHelper.RandomGender();
	    string name = NameGenerator.Generate(gender);
	    ActorInventory.InvContents inv = new ActorInventory.InvContents();

	    return new ActorData(ActorRegistry.GetUnusedId(name),
		    name,
		    personality,
		    race,
		    gender,
		    null,
		    new ActorPhysicalCondition(),
		    inv,
		    new FactionStatus(null));
    }
}
