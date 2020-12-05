using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

// Manages the log of all previous world events
public class History : MonoBehaviour
{
	public const string TraderArrivalEvent = "trader_arrives";
	public const string NewSettlerEvent = "new_settler";

	private EventLog eventLog; // Stores all world events in reverse chronological order

	public EventLog GetEventLog()
	{
		return eventLog;
	}

	public void LoadEventLog (EventLog events)
	{
		if (events == null)
		{
			events = new EventLog();
			Debug.LogWarning("Tried to load null event log.");
		}
		eventLog = events;
	}

	public void LogEvent(string eventId)
	{
		if (eventLog == null)
		{
			eventLog = new EventLog();
		}
		Event newEvent = new Event(eventId, TimeKeeper.CurrentTick);
		eventLog.events.AddFirst(newEvent);
	}

	public Event GetMostRecent(string eventId)
	{
		foreach(Event logEvent in eventLog.events)
		{
			if (logEvent.id == eventId)
			{
				return logEvent;
			}
		}
		return null;
	}

	public class EventLog
	{
		public LinkedList<Event> events;

		public EventLog()
		{
			events = new LinkedList<Event>();
		}
	}

	public class Event
	{
		public string id;
		public ulong time;

		public Event(string id, ulong time)
		{
			this.id = id;
			this.time = time;
		}
	}
}
