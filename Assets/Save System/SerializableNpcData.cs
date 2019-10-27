﻿using System.Collections;
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

		invContents = new SerializableActorInv(sourceInv);
	}
}

public static class SerializableNpcDataExtension
{
	public static NPCData ToNonSerializable (this SerializableNpcData source)
	{
		NPCData retVal = new NPCData(source.npcId, source.npcName, source.bodySprite, source.hairId, source.gender, source.schedule, source.relationships);
		return retVal;
	}

	public static ActorInventory.InvContents ToNonSerializable (this SerializableActorInv source)
	{
		ActorInventory.InvContents newInv = new ActorInventory.InvContents();
		if (source.mainInv == null)
		{
			source.mainInv = new string[0];
		}
		if (source.hotbar == null)
		{
			source.hotbar = new string[0];
		}
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