using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeKeeper : MonoBehaviour {

	public delegate void TimeEvent ();
	public static event TimeEvent OnSecondChanged;
	public static event TimeEvent OnMinuteChanged;

	static int currentTime;
	static WeekDay currentDay;
	static int lastSecondCount;

	// Rate of in-game seconds for every real second
	static float timeSpeed = 40f;

	void OnDestroy ()
	{
		OnSecondChanged = null;
		OnMinuteChanged = null;
	}
	void Start () {
		// format HHMMSS
		currentTime = 090600;
		currentDay = WeekDay.Wednesday;
	}
	void Update () {
		if (Mathf.FloorToInt(Time.time * timeSpeed) > lastSecondCount) {
			lastSecondCount = Mathf.FloorToInt (Time.time * timeSpeed);
			IncrementSeconds ();
		}
	}

	static void IncrementSeconds () {
		currentTime += 1;
		// increment minute
		if (currentTime % 100 >= 60) {
			currentTime -= currentTime % 100;
			currentTime += 100;
			if (OnMinuteChanged != null)
				OnMinuteChanged ();
		}
		// increment hour
		if ((currentTime % 10000) / 100 >= 60 ) {
			currentTime -= (currentTime % 10000);
			currentTime += 10000;
		}
		// increment day
		if (currentTime / 10000 >= 24) {
			currentTime = 0;
			currentDay = WeekDayMethods.GetNextDay (currentDay);
		}
		if (OnSecondChanged != null)
			OnSecondChanged ();
	}
	public static int RawTime {
		get { return currentTime; }
	}
	public static string FormattedTime {
		get {
			int min = (currentTime % 10000) / 100;
			int hour = currentTime / 10000;
			bool isPm = (hour >= 12);
			hour %= 12;
			if (hour == 0)
				hour = 12;
			if (isPm)
				return (hour + ":" + min.ToString("00") + " pm");
			else
				return (hour + ":" + min.ToString("00") + " am");
		}
	}
	public static WeekDay DayOfWeek {
		get { return currentDay; }
	}
}
