using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCData {
    public string NpcName { get; private set; }
    public string NpcId { get; private set; }
    public string BodySprite { get; private set; }
    public string HairId { get; private set; }
    public string HatId { get; private set; }
    public string ShirtId { get; private set; }
    public string PantsId { get; private set; }
    public Gender Gender { get; private set; }
    public List<Relationship> Relationships { get; private set; }
    public List<ScheduleEvent> Schedule { get; private set; }

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

	public NPCData (string id, string name, string bodySprite, Gender gender) {
		this.NpcName = name;
		this.NpcId = id;
		this.BodySprite = bodySprite;
		this.Gender = gender;
		this.Schedule = new List<ScheduleEvent> ();
		this.Relationships = new List<Relationship> ();
	}
	public NPCData (string id, string name, string bodySprite, string hatId, string shirtId, string pantsId, Gender gender) {
		this.NpcName = name;
		this.NpcId = id;
		this.BodySprite = bodySprite;
		this.HatId = hatId;
		this.ShirtId = shirtId;
		this.PantsId = pantsId;
		this.Gender = gender;
		this.Schedule = new List<ScheduleEvent> ();
		this.Relationships = new List<Relationship> ();
	}
	public NPCData (string id, string name, string bodySprite, Gender gender, List<ScheduleEvent> schedule) {
		this.NpcName = name;
		this.NpcId = id;
		this.BodySprite = bodySprite;
		this.Gender = gender;
		this.Schedule = schedule;
		this.Relationships = new List<Relationship> ();
	}
	public NPCData (string id, string name, string bodySprite, Gender gender, List<Relationship> relationships) {
		this.NpcName = name;
		this.NpcId = id;
		this.BodySprite = bodySprite;
		this.Gender = gender;
		this.Schedule = new List<ScheduleEvent> ();
		this.Relationships = relationships;
	}
	public NPCData (string id, string name, string bodySprite, Gender gender, List<ScheduleEvent> schedule, List<Relationship> relationships) {
		this.NpcName = name;
		this.NpcId = id;
		this.BodySprite = bodySprite;
		this.Gender = gender;
		this.Schedule = schedule;
		this.Relationships = relationships;
	}
	public NPCData (string id, string name, string bodySprite, string hairId, string hatId, string shirtId, string pantsId, Gender gender, List<ScheduleEvent> schedule, List<Relationship> relationships) {
		this.NpcName = name;
		this.NpcId = id;
		this.BodySprite = bodySprite;
        this.HairId = hairId;
		this.HatId = hatId;
		this.ShirtId = shirtId;
		this.PantsId = pantsId;
		this.Gender = gender;
		this.Schedule = schedule;
		this.Relationships = relationships;
	}
	public void SetRelationship (string id, float value) {
		for (int i = 0; i < this.Relationships.Count; i++)
			if (Relationships[i].id == id) {
                Relationships[0].value = value;
				return;
			}
		this.Relationships.Add (new Relationship (id, value));
		return;
	}
	// TODO a way to define an NPC's daily routine
}

