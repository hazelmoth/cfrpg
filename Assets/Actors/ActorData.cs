
// Stores all the data associated with an actor's identity (but not their physical location)

using System.Collections.Generic;

public class ActorData
{
	public ActorData(string actorId, string actorName, string personality, string race, Gender gender, string hair, ActorPhysicalCondition physicalCondition, ActorInventory.InvContents inventory, FactionStatus factionStatus)
	{
		this.actorId = actorId;
		ActorName = actorName;
		Personality = personality;
		Race = race;
		Gender = gender;
		Hair = hair;
		PhysicalCondition = physicalCondition;
		Inventory = new ActorInventory();
		Inventory.SetInventory(inventory);
		FactionStatus = factionStatus;
	}

	public readonly string actorId;
	public string ActorName { get; set;  }
	public string Personality { get; set; }
	public string Race { get; set; }
	public Gender Gender { get; set; }
	public string Hair { get; set; }
	public ActorPhysicalCondition PhysicalCondition { get; }
	public ActorInventory Inventory { get; set; }
	public FactionStatus FactionStatus { get; }
	public ActorLocationMemories Memories { get; }
	public List<Relationship> Relationships { get; }

	[System.Serializable]
	public struct Relationship
	{
		public string id;
		public float value;
		public Relationship(string id, float val)
		{
			this.id = id;
			this.value = val;
		}
	}

}
