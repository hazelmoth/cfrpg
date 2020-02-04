using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCGenerator : MonoBehaviour
{
	static System.Random random;
    public static NPCData Generate ()
    {
        List<Hair> hairPool = HairLibrary.GetHairs();
        List<Hat> hatPool = ContentLibrary.Instance.Items.GetHats();
        List<Shirt> shirtPool = ContentLibrary.Instance.Items.GetShirts();
        List<Pants> pantsPool = ContentLibrary.Instance.Items.GetPants();

        Hair hair = hairPool.PickRandom();
        Hat hat = hatPool.PickRandom();
        Shirt shirt = shirtPool.PickRandom();
        Pants pants = pantsPool.PickRandom();

        string hatId = hat.ItemId;

		// 50% chance of no hat 
		if (random == null)
			random = new System.Random();
        if (random.Next(2) == 0)
            hatId = null;


        Gender gender = GenderHelper.RandomGender();
        string name = NameGenerator.Generate(gender);
		ActorInventory.InvContents inv = new ActorInventory.InvContents();
		inv.equippedHat = hat;
		inv.equippedShirt = shirt;
		inv.equippedPants = pants;

        return new NPCData(NPCDataMaster.GetUnusedId(name), name, "human_light", hair.hairId, gender, inv);
    }

}
