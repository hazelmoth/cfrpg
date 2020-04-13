using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// All the data that defines a specific NPC.
// This used to store information and spawn NPCs; it's not stored on NPCs.
public class NPCData
{
	public string NpcName { get; private set; }
	public string NpcId { get; private set; }
	public string BodySprite { get; private set; }
	public string HairId { get; private set; }
	public string Personality { get; private set; }
	public Gender Gender { get; private set; }
	public ActorInventory.InvContents Inventory { get; private set; }
    public List<Relationship> Relationships { get; private set; }
    public List<ScheduleEvent> Schedule { get; private set; }

	[System.Serializable]
    public struct Relationship {
		public string id;
		public float value;
		public Relationship (string id, float val) {
			this.id = id;
			this.value = val;
		}
	}
	[System.Serializable]
	public struct ScheduleEvent {
		public int startTime;
		public List<WeekDay> days;
		public string eventId;
		public ScheduleEvent (int startTime, List<WeekDay> days, string eventId) {
			this.startTime = startTime;
			this.days = days;
			this.eventId = eventId;
		}
	}

	public NPCData (string id, string name, string bodySprite, Gender gender) {
		this.NpcName = name;
		this.NpcId = id;
		this.BodySprite = bodySprite;
		this.Gender = gender;
		this.Schedule = new List<ScheduleEvent> ();
		this.Relationships = new List<Relationship> ();
	}
	public NPCData(string id, string name, string bodySprite, string hairId, Gender gender, string personality, ActorInventory.InvContents inventory)
	{
		this.NpcName = name;
		this.NpcId = id;
		this.BodySprite = bodySprite;
		this.HairId = hairId;
		this.Gender = gender;
		this.Personality = personality;
		this.Inventory = inventory;
		this.Schedule = new List<ScheduleEvent>();
		this.Relationships = new List<Relationship>();
	}

	public void SetRelationship (string id, float value) {
		for (int i = 0; i < this.Relationships.Count; i++)
			if (Relationships[i].id == id) {
				Relationships[i] = (new Relationship(id, value));
				return;
			}
		this.Relationships.Add (new Relationship (id, value));
		return;
	}
}

