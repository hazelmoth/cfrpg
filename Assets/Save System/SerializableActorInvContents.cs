using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SerializableActorInv
{
	public string[] hotbar;
	public string[] mainInv;
	public string hat;
	public string shirt;
	public string pants;

	public SerializableActorInv (ActorInventory.InvContents sourceInv)
	{
		mainInv = new string[sourceInv.mainInvArray.Length];
		hotbar = new string[sourceInv.hotbarArray.Length];

		for (int i = 0; i < sourceInv.mainInvArray.Length; i++)
		{
			mainInv[i] = sourceInv.mainInvArray[i] != null ? sourceInv.mainInvArray[i].GetItemId() : string.Empty;
		}
		for (int i = 0; i < sourceInv.hotbarArray.Length; i++)
		{
			hotbar[i] = sourceInv.hotbarArray[i] != null ? sourceInv.hotbarArray[i].GetItemId() : string.Empty;
		}

		hat = sourceInv.equippedHat != null ? sourceInv.equippedHat.ItemId : string.Empty;
		shirt = sourceInv.equippedShirt != null ? sourceInv.equippedShirt.ItemId : string.Empty;
		pants = sourceInv.equippedPants != null ? sourceInv.equippedPants.ItemId : string.Empty;
	}
}
