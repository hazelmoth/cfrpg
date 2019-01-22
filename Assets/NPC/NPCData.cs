using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCData {
	string npcId;
	string spriteName;
	string npcName;
	string gender;
	List<Relationship> relationships;
	List<ScheduleEvent> schedule;

	public string NpcName {get {return npcName;}}
	public string NpcId {get {return npcId;}}
	public string SpriteName {get {return spriteName;}}
	public string Gender {get {return gender;}}
	public List<Relationship> Relationships {get {return relationships;}}
	public List<ScheduleEvent> Schedule {get {return schedule;}}

	public class Relationship {
		public string id;
		public float value;
		public Relationship (string id, float val) {
			this.id = id;
			this.value = val;
		}
	}
	public class ScheduleEvent {
		public int startTime;
		public List<WeekDay> days;
		public string eventId;
		public ScheduleEvent (int startTime, List<WeekDay> days, string eventId) {
			this.startTime = startTime;
			this.days = days;
			this.eventId = eventId;
		}
	}

	public NPCData (string id, string name, string spriteName, string gender) {
		this.npcName = name;
		this.npcId = id;
		this.spriteName = spriteName;
		this.gender = gender;
		this.schedule = new List<ScheduleEvent> ();
		this.relationships = new List<Relationship> ();
	}
	public NPCData (string id, string name, string spriteName, string gender, List<ScheduleEvent> schedule) {
		this.npcName = name;
		this.npcId = id;
		this.gender = gender;
		this.schedule = schedule;
		this.relationships = new List<Relationship> ();
	}
	public NPCData (string id, string name, string spriteName, string gender, List<Relationship> relationships) {
		this.npcName = name;
		this.npcId = id;
		this.gender = gender;
		this.schedule = new List<ScheduleEvent> ();
		this.relationships = relationships;
	}
	public NPCData (string id, string name, string spriteName, string gender, List<ScheduleEvent> schedule, List<Relationship> relationships) {
		this.npcName = name;
		this.npcId = id;
		this.gender = gender;
		this.schedule = schedule;
		this.relationships = relationships;
	}
	public void SetRelationship (string id, float value) {
		for (int i = 0; i < this.relationships.Count; i++)
			if (relationships[i].id == id) {
				relationships[0].value = value;
				return;
			}
		this.relationships.Add (new Relationship (id, value));
		return;
	}
	// TODO a way to define an NPC's daily routine
}

