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
	public ActorInventory.InvContents invContents;
	public ActorPhysicalCondition condition;
	public FactionStatus faction;
	public List<ActorData.Relationship> relationships;

	public SerializableActorData (ActorData source)
	{
		actorName = source.ActorName;
		actorId = source.actorId;
		bodySprite = source.Race;
		hairId = source.Hair;
		gender = source.Gender;
		relationships = source.Relationships;
		personality = source.Personality;
		invContents = source.Inventory.GetContents();
	}
}

public static class SerializableActorDataExtension
{
	public static ActorData ToNonSerializable (this SerializableActorData source)
	{
		ActorInventory.InvContents deserizalizedInv = new ActorInventory.InvContents();

		// Go through and turn any items with blank ids into null (because the serializer stores empty indices as blank items (cuz its a fuckin idiot)).

		for (int i = 0; i < source.invContents.mainInvArray.Length; i++)
		{
			deserizalizedInv.mainInvArray[i] = source.invContents.mainInvArray[i];
			if (string.IsNullOrEmpty(source.invContents.mainInvArray[i].id))
			{
				deserizalizedInv.mainInvArray[i] = null;
			}
		}
		for (int i = 0; i < source.invContents.hotbarArray.Length; i++)
		{
			deserizalizedInv.hotbarArray[i] = source.invContents.hotbarArray[i];
			if (string.IsNullOrEmpty(source.invContents.hotbarArray[i].id))
			{
				deserizalizedInv.hotbarArray[i] = null;
			}
		}
		deserizalizedInv.equippedHat = string.IsNullOrEmpty(source.invContents.equippedHat.id) ? null : source.invContents.equippedHat;
		deserizalizedInv.equippedShirt = string.IsNullOrEmpty(source.invContents.equippedShirt.id) ? null : source.invContents.equippedShirt;
		deserizalizedInv.equippedPants = string.IsNullOrEmpty(source.invContents.equippedPants.id) ? null : source.invContents.equippedPants;


		ActorData retVal = new ActorData(source.actorId, source.actorName, source.personality, source.bodySprite, source.gender, source.hairId, source.condition, deserizalizedInv, source.faction);
		return retVal;
	}
}
