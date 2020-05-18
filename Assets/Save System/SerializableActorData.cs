using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableActorData
{
	public string actorName;
	public string actorId;
	public string bodySprite;
	public string personality;
	public string hairId;
	public Gender gender;
	public SerializableActorInv invContents;
	public ActorPhysicalCondition condition;
	public FactionStatus faction;
	public List<ActorData.Relationship> relationships;

	public SerializableActorData (ActorData source, ActorInventory.InvContents sourceInv)
	{
		actorName = source.ActorName;
		actorId = source.actorId;
		bodySprite = source.Race;
		hairId = source.Hair;
		gender = source.Gender;
		relationships = source.Relationships;
		personality = source.Personality;

		invContents = new SerializableActorInv(sourceInv);
	}
}

public static class SerializableActorDataExtension
{
	public static ActorData ToNonSerializable (this SerializableActorData source)
	{
		ActorData retVal = new ActorData(source.actorId, source.actorName, source.personality, source.bodySprite, source.gender, source.hairId, source.condition, source.invContents.ToNonSerializable(), source.faction);
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
			newInv.mainInvArray[i] = source.mainInv[i] == string.Empty ? null : ContentLibrary.Instance.Items.Get(source.mainInv[i]);
		}
		for (int i = 0; i < source.hotbar.Length; i++)
		{
			newInv.hotbarArray[i] = source.hotbar[i] == string.Empty ? null : ContentLibrary.Instance.Items.Get(source.hotbar[i]);
		}

		newInv.equippedHat = source.hat == string.Empty ? null : ContentLibrary.Instance.Items.Get(source.hat);
		newInv.equippedShirt = source.shirt == string.Empty ? null : ContentLibrary.Instance.Items.Get(source.shirt);
		newInv.equippedPants = source.pants == string.Empty ? null : ContentLibrary.Instance.Items.Get(source.pants);

		return newInv;
	}
}
