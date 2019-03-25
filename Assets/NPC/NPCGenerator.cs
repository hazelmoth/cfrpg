using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCGenerator : MonoBehaviour
{
    static NPCData Generate ()
    {
        List<Hair> hairPool = HairLibrary.GetHairs();
        List<Hat> hatPool = ItemManager.GetHats();
        List<Shirt> shirtPool = ItemManager.GetShirts();
        List<Pants> pantsPool = ItemManager.GetPants();

        Hair hair = hairPool.PickRandom();
        Hat hat = hatPool.PickRandom();
        Shirt shirt = shirtPool.PickRandom();
        Pants pants = pantsPool.PickRandom();

        Gender gender = GenderHelper.RandomGender();
        string name = NameGenerator.Generate(gender);
        string id = name.ToLower().Replace(' ', '_');
        return new NPCData(id, name, "human_base", hat.itemId, shirt.itemId, pants.itemId, gender);
    }

}
