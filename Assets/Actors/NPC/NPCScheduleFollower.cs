using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Responsible for choosing which tasks the NPC performs, based on its schedule.
public class NPCScheduleFollower : MonoBehaviour {

	List<NPCData.ScheduleEvent> schedule;
	NPCTaskExecutor executor;

	// Use this for initialization
	void Start () {
		schedule = NPCDataMaster.GetNpcFromId (GetComponent<NPC> ().NpcId).Schedule;
		executor = GetComponent<NPCTaskExecutor> ();
		TimeKeeper.OnSecondChanged += CheckSchedule;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void CheckSchedule () {
		foreach (NPCData.ScheduleEvent item in schedule) {
			if (item.startTime == TimeKeeper.RawTime && item.days.Contains(TimeKeeper.DayOfWeek)) {
				ParseAndExecute (item.eventId);
			}
		}
	}
	void ParseAndExecute (string scheduleItemId) {
		switch (scheduleItemId) {
		case "wander":
			executor.Wander ();
			return;
		default:
			Debug.LogWarning (
				"NPCScheduleFollower was passed a schedule item called + \"" + scheduleItemId + 
				"\", which isn't something it knows how to handle.");
			return;
		}
	}
}
