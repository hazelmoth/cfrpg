using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeKeeper : MonoBehaviour {

	public struct DateTime
	{
		public int rawTime;
		public int day;
		public int year;
		public WeekDay weekDay;
	}

	public delegate void TimeEvent();
	public static event TimeEvent OnSecondChanged;
	public static event TimeEvent OnMinuteChanged;

	// The elapsed seconds on the current day
	private static int time;

	private static int day;
	private static int year;
	private static WeekDay weekDay;
	private static int lastSecondCount;

	private static int Second => time % 60;
	private static int Min => (time % 3600) / 60;
	private static int Hour => time / 3600;
	private static bool IsPm => Hour >= 12;

	// Rate of in-game seconds for every real second
	public static float timeScale = 48f;

	public static float TimeAsFraction => time / 86400f;

	public static WeekDay DayOfWeek => weekDay;

	private void OnDestroy()
	{
		OnSecondChanged = null;
		OnMinuteChanged = null;
	}

	private void Start() {
		weekDay = WeekDay.Wednesday;
		day = 3;
		year = 2202;
		SetTime(0.5f);
	}

	private void Update() {
		if (Mathf.FloorToInt(Time.time * timeScale) > lastSecondCount) {
			int currentSecondCount = Mathf.FloorToInt(Time.time * timeScale);
			IncrementSeconds(currentSecondCount - lastSecondCount);
			lastSecondCount = currentSecondCount;
		}
	}

	public static void SetTime(float timeAsFraction)
	{
		timeAsFraction = Mathf.Clamp01(timeAsFraction);

		time = (int)(timeAsFraction * 86400);
	}

	public static int RawTime
	{
		get { return time; }
	}
	public static DateTime CurrentDateTime
	{
		get
		{

			return new DateTime
			{
				rawTime = time,
				day = day,
				year = year,
				weekDay = weekDay
			};
		}
	}

	public static string FormattedTime
	{
		get
		{
			int min = Min;
			int hour = Hour;
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
	public static float daysBetween (DateTime first, DateTime second)
	{
		float result = 0;
		result += (second.year - first.year) * Calendar.DaysInYear;
		result += (second.day - first.day);
		result += (second.rawTime - first.rawTime) / 86400f;
		return Mathf.Abs(result);
	}

	public static void IncrementDay()
	{
		weekDay = WeekDayMethods.GetNextDay(weekDay);
		day++;
		if (day >= Calendar.DaysInYear)
		{
			day = 0;

			year++;
			Debug.Log("it's now year " + year);
			Debug.Log("it's now the month of " + Calendar.GetMonth(day));
		}
	}

	private static void IncrementSeconds (int secondsToAdd) {
		time += secondsToAdd;

		// increment day
		if (time >= 86400) {
			time = 0;
			IncrementDay();
		}
		OnSecondChanged?.Invoke();
	}
}
