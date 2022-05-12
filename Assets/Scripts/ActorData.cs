
// Stores all the data associated with an actor's identity (but not their physical location)

using System.Collections.Generic;
using ContentLibraries;

public class ActorData
{
	public ActorData(
		string actorId,
		string actorName,
		string personality,
		string raceId,
		Gender gender,
		string hair,
		ActorHealth health,
		ActorInventory.InvContents inventory,
		int walletMoney,
		int debt,
		FactionStatus factionStatus,
		string role)
	{
		ActorId = actorId;
		ActorName = actorName;
		Personality = personality;
		RaceId = raceId;
		Gender = gender;
		Hair = hair;
		Inventory = new ActorInventory();
		Inventory.SetInventory(inventory ?? new ActorInventory.InvContents());
		Wallet = new ActorWallet(walletMoney);
		CurrentDebt = debt;
		FactionStatus = factionStatus ?? new FactionStatus(null);
		Role = role;

		IActorRace race = ContentLibrary.Instance.Races.Get(this.RaceId);
		Health = health ?? new ActorHealth(race.MaxHealth, race.MaxHealth);
	}

	public string ActorId { get; }
	public string ActorName { get; set; }
	public string Personality { get; set; }
	public string RaceId { get; set; }
	public string Hair { get; set; }
	public Gender Gender { get; }
	public ActorHealth Health { get; }
	public ActorInventory Inventory { get; }
	public ActorWallet Wallet { get; }
	public int CurrentDebt { get; set; }
	public FactionStatus FactionStatus { get; }
	public List<Relationship> Relationships { get; }
	public string Role { get; set; }

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
