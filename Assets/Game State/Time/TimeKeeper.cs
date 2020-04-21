using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeKeeper : MonoBehaviour {

	public delegate void TimeEvent ();
	public static event TimeEvent OnSecondChanged;
	public static event TimeEvent OnMinuteChanged;

	// format HHMMSS
	static int currentTime;
	static int currentDate;
	static int currentMonth;
	static int currentYear;
	static WeekDay currentDay;
	static int lastSecondCount;

	static int Second => currentTime % 100;
	static int Min => (currentTime % 10000) / 100;
	static int Hour => currentTime / 10000;
	static bool IsPm => Hour >= 12;

	// Rate of in-game seconds for every real second
	public static float timeScale = 48f;

	void OnDestroy ()
	{
		OnSecondChanged = null;
		OnMinuteChanged = null;
	}
	void Start () {
		// format HHMMSS
		currentTime = 090600;
		currentDay = WeekDay.Wednesday;
		currentDate = 3;
		currentMonth = 1;
		currentYear = 1999;
	}
	void Update () {
		if (Mathf.FloorToInt(Time.time * timeScale) > lastSecondCount) {
			int currentSecondCount = Mathf.FloorToInt (Time.time * timeScale);
			IncrementSeconds (currentSecondCount - lastSecondCount);
			lastSecondCount = currentSecondCount;
		}
	}

	static void IncrementSeconds ()
	{
		IncrementSeconds(1);
	}
	static void IncrementSeconds (int secondsToAdd) {
		currentTime += secondsToAdd;
		// increment minute
		if (currentTime % 100 >= 60) {
			currentTime -= currentTime % 100;
			currentTime += 100;
			OnMinuteChanged?.Invoke();
		}
		// increment hour
		if ((currentTime % 10000) / 100 >= 60 ) {
			currentTime -= (currentTime % 10000);
			currentTime += 10000;
		}
		// increment day
		if (currentTime / 10000 >= 24) {
			currentTime = 0;
			IncrementDay();
		}
		OnSecondChanged?.Invoke();
	}
	static void IncrementDay ()
	{
		currentDay = WeekDayMethods.GetNextDay(currentDay);
		currentDate++;
		if (currentDate >= Calendar.Months[currentMonth].NumDays)
		{
			currentDate = 0;
			currentMonth++;
			if (currentMonth >= Calendar.Months.Count)
			{
				currentMonth = 0;
				currentYear++;
				Debug.Log("it's now year " + currentYear);
				Debug.Log("it's now the month of " + Calendar.Months[currentMonth].Name);
			}
			else
			{
				Debug.Log("it's now the month of " + Calendar.Months[currentMonth].Name);
			}
		}
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
	public static float TimeAsFraction
	{
		get
		{
			float exactHours = Hour;
			exactHours += (float)Min / 60;
			exactHours += (float)Second / 3600;
			exactHours += (Time.time * timeScale - lastSecondCount) / 3600;
			return exactHours / 24f;
		}
	}
	public static WeekDay DayOfWeek {
		get { return currentDay; }
	}
}
