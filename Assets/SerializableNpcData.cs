using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableNpcData
{
	public string npcName;
	public string npcId;
	public string bodySprite;
	public string hairId;
	public Gender gender;
	public SerializableActorInv invContents;
	public List<NPCData.Relationship> relationships;
	public List<NPCData.ScheduleEvent> schedule;

	public SerializableNpcData (NPCData source, ActorInventory.InvContents sourceInv)
	{
		npcName = source.NpcName;
		npcId = source.NpcId;
		bodySprite = source.BodySprite;
		hairId = source.HairId;
		gender = source.Gender;
		relationships = source.Relationships;
		schedule = source.Schedule;

		invContents = new SerializableActorInv
		{
			mainInv = new string[sourceInv.mainInvArray.Length],
			hotbar = new string[sourceInv.hotbarArray.Length]
		};

		for (int i = 0; i < sourceInv.mainInvArray.Length; i++)
		{
			invContents.mainInv[i] = sourceInv.mainInvArray[i] != null ? sourceInv.mainInvArray[i].GetItemId() : string.Empty;
		}
		for (int i = 0; i < sourceInv.hotbarArray.Length; i++)
		{
			invContents.hotbar[i] = sourceInv.hotbarArray[i] != null ? sourceInv.hotbarArray[i].GetItemId() : string.Empty;
		}

		invContents.hat = sourceInv.equippedHat != null ? sourceInv.equippedHat.ItemId : string.Empty;
		invContents.shirt = sourceInv.equippedShirt != null ? sourceInv.equippedShirt.ItemId : string.Empty;
		invContents.pants = sourceInv.equippedPants != null ? sourceInv.equippedPants.ItemId : string.Empty;
	}
}

public static class SerializableNpcDataExtension
{
	public static NPCData ToNonSerializable (this SerializableNpcData source)
	{
		NPCData retVal = new NPCData(source.npcId, source.npcName, source.bodySprite, source.gender, source.schedule, source.relationships);
		return retVal;
	}

	public static ActorInventory.InvContents ToNonSerializable (this SerializableActorInv source)
	{
		ActorInventory.InvContents newInv = new ActorInventory.InvContents();

		for (int i = 0; i < source.mainInv.Length; i++)
		{
			newInv.mainInvArray[i] = source.mainInv[i] == string.Empty ? null : ItemManager.GetItemById(source.mainInv[i]);
		}
		for (int i = 0; i < source.hotbar.Length; i++)
		{
			newInv.hotbarArray[i] = source.hotbar[i] == string.Empty ? null : ItemManager.GetItemById(source.hotbar[i]);
		}

		newInv.equippedHat = source.hat == string.Empty ? null : ItemManager.GetItemById(source.hat);
		newInv.equippedShirt = source.shirt == string.Empty ? null : ItemManager.GetItemById(source.shirt);
		newInv.equippedPants = source.pants == string.Empty ? null : ItemManager.GetItemById(source.pants);

		return newInv;
	}
}
