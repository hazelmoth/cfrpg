using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NPCDataParser {

	public static List<NPCData> Parse (string jsonString) {
		List<NPCData> list = new List<NPCData> ();
		SimpleJSON.JSONNode json = SimpleJSON.JSON.Parse (jsonString);
		for (int i = 0; i < JSONHelper.GetElementCount(json); i++) {
			string npcId;
			string spriteName;
            string hairId;
			string hatId;
			string shirtId;
			string pantsId;
			string npcName;
			string npcGender;
			List<NPCData.Relationship> relationships;
			List<NPCData.ScheduleEvent> schedule;
			npcName = json [i] ["name"];
			npcId = json [i] ["id"];
			spriteName = json [i] ["sprite"];
            hairId = json[i]["hair"];
			hatId = json [i] ["hat"];
			shirtId = json [i] ["shirt"];
			pantsId = json [i] ["pants"];
			npcGender = json [i] ["gender"];

			relationships = new List<NPCData.Relationship> ();
			for (int j = 0; j < JSONHelper.GetElementCount(json[i]["relationships"]); j++) {
				SimpleJSON.JSONNode relJson = json [i] ["relationships"] [j];
				relationships.Add (new NPCData.Relationship(relJson["id"], relJson["value"]));
			}

			schedule = new List<NPCData.ScheduleEvent> ();
			for (int j = 0; j < JSONHelper.GetElementCount(json[i]["schedule"]); j++) {
				SimpleJSON.JSONNode itemJson = json [i] ["schedule"] [j];
				int startTime = itemJson ["startTime"];
				string eventId = itemJson ["action"];
				List<WeekDay> days = new List<WeekDay> ();
				for (int k = 0; k < JSONHelper.GetElementCount(itemJson["days"]); k++) {
					days.Add (WeekDayMethods.WeekdayFromString (itemJson ["days"] [k]));
				}
				schedule.Add (new NPCData.ScheduleEvent(startTime, days, eventId));
			}
			NPCData npc = new NPCData(
				npcId,
				npcName,
				spriteName,
				hairId,
				GenderHelper.GenderFromString(npcGender),
				schedule,
				relationships
			);
			list.Add (npc);
		}
		return list;
	}
}
