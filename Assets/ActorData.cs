
// Stores all the data associated with an actor's identity (but not their physical location)
public class ActorData
{
	public ActorData(string actorId, string actorName, string personality, string race, string hair, ActorPhysicalCondition physicalCondition, ActorInventory inventory, FactionStatus factionStatus)
	{
		this.actorId = actorId;
		ActorName = actorName;
		Personality = personality;
		Race = race;
		Hair = hair;
		PhysicalCondition = physicalCondition;
		Inventory = inventory;
		FactionStatus = factionStatus;
	}

	public readonly string actorId;
	public string ActorName { get; set;  }
	public string Personality { get; set; }
	public string Race { get; set; }
	public string Hair { get; set; }
	public ActorPhysicalCondition PhysicalCondition { get; }
	public ActorInventory Inventory { get; set; }
	public FactionStatus FactionStatus { get; }

}
