using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCGenerator : MonoBehaviour
{
    public static NPCData Generate ()
    {
        List<Hair> hairPool = HairLibrary.GetHairs();
        List<Hat> hatPool = ItemManager.GetHats();
        List<Shirt> shirtPool = ItemManager.GetShirts();
        List<Pants> pantsPool = ItemManager.GetPants();

        Hair hair = hairPool.PickRandom();
        Debug.Log(hair);
        Hat hat = hatPool.PickRandom();
        Shirt shirt = shirtPool.PickRandom();
        Pants pants = pantsPool.PickRandom();

        string hatId = hat.itemId;

        // 50% chance of no hat 
        if (Random.Range(0, 2) == 0)
            hatId = null;


        Gender gender = GenderHelper.RandomGender();
        string name = NameGenerator.Generate(gender);
        string id = name.ToLower().Replace(' ', '_');
        return new NPCData(id, name, "human_base", hair.hairId, hatId, shirt.itemId, pants.itemId, gender);
    }

}
