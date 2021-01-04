using Newtonsoft.Json;
using System.Collections.Generic;

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
	public int money;
	public ActorPhysicalCondition condition;
	public FactionStatus faction;
	public List<ActorData.Relationship> relationships;
	public string profession;

	[JsonConstructor]
	public SerializableActorData () { }

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
		money = source.Wallet.Balance;
		profession = source.Profession;
	}
}

public static class SerializableActorDataExtension
{
	public static ActorData ToNonSerializable (this SerializableActorData source)
	{

		ActorInventory.InvContents deserizalizedInv = new ActorInventory.InvContents();

		for (int i = 0; i < source.invContents.mainInvArray.Length; i++)
		{
			deserizalizedInv.mainInvArray[i] = source.invContents.mainInvArray[i];
		}
		for (int i = 0; i < source.invContents.hotbarArray.Length; i++)
		{
			deserizalizedInv.hotbarArray[i] = source.invContents.hotbarArray[i];
		}
		deserizalizedInv.equippedHat = source.invContents.equippedHat;
		deserizalizedInv.equippedShirt = source.invContents.equippedShirt;
		deserizalizedInv.equippedPants = source.invContents.equippedPants;


		ActorData retVal = new ActorData(source.actorId, source.actorName, source.personality, source.bodySprite, source.gender, source.hairId, source.condition, deserizalizedInv, source.money, source.faction, source.profession);
		return retVal;
	}
}
