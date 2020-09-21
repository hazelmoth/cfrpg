using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TimeKeeper : MonoBehaviour {

	public struct DateTime
	{
		public int seconds;
		public int day;
		public int year;
		public WeekDay weekDay;

		public override string ToString()
		{
			int min = (seconds % 3600) / 60;
			int hour = (seconds / secondsPerDay) * 24;

			bool isPm = (hour >= 12);
			hour %= 12;
			if (hour == 0)
				hour = 12;
			string timeString;
			if (isPm)
				timeString = (hour + ":" + min.ToString("00") + " pm");
			else
				timeString = (hour + ":" + min.ToString("00") + " am");

			return weekDay.ToString() + ", " + Calendar.GetMonth(day).Name + " " + Calendar.GetDayOfMonth(day) + " " + year + ", " + timeString;
		}
	}

	public delegate void TimeEvent();
	public static event TimeEvent OnMinuteChanged;

	private const int ticksPerSecond = 60; // The number of ticks in a real second, at normal timescale
	private const int secondsPerDay = 86400; // Number of ingame seconds in an ingame day
	private const int clockStartYear = 2000; // The year that ticks count up from

	private static ulong lifetimeTicks; // Number of ticks since the clock start year at 12:00:00 am

	private static uint lastTickCount; // The number of ticks since game launch, as of the previous frame
									   // (where a tick is 1/60 seconds).

	private static float TicksPerIngameSecond => ticksPerSecond / timeScale;
	private static double LifetimeSeconds => (double)lifetimeTicks / TicksPerIngameSecond;
	private static double LifetimeDays => LifetimeSeconds / secondsPerDay;
	private static int LifetimeYears => (int)(LifetimeDays / (ulong)Calendar.DaysInYear);

	private static uint TicksToday => (uint)(lifetimeTicks % (TicksPerIngameSecond * secondsPerDay)); // How many ticks have elapsed on the current day

	private static int Year => LifetimeYears + clockStartYear;
	private static int Day => (int)(LifetimeDays % (uint)Calendar.DaysInYear);
	private static int Second => (int)(LifetimeSeconds % 60);
	private static int Min => (int)(LifetimeSeconds % 3600) / 60;
	private static int Hour => (int)(LifetimeSeconds % secondsPerDay) / 3600;


	// Number of in-game seconds for every real second
	public static float timeScale = 48f;

	public static ulong CurrentTick => lifetimeTicks;

	public static float TimeAsFraction => TicksToday / (TicksPerIngameSecond * secondsPerDay);

	public static WeekDay DayOfWeek => WeekDayHelper.FromInt((int)(LifetimeDays % (ulong)WeekDayHelper.DaysOfWeek));

	private void OnDestroy()
	{
		OnMinuteChanged = null;
	}

	private void Start() {
		SetTime(0.5f);
		lastTickCount = (uint)Mathf.FloorToInt(Time.time * ticksPerSecond);
	}

	private void Update() {
		int oldMin = Min;
		uint tickCount = (uint)Mathf.FloorToInt(Time.time * ticksPerSecond);
		lifetimeTicks += tickCount - lastTickCount;
		lastTickCount = tickCount;
		if (Min != oldMin)
		{
			OnMinuteChanged?.Invoke();
		}
	}

	public static void SetTime(float timeAsFraction)
	{
		timeAsFraction = Mathf.Clamp01(timeAsFraction);

		ulong newTicksToday = (ulong)(timeAsFraction * secondsPerDay * TicksPerIngameSecond);

		lifetimeTicks -= TicksToday;
		lifetimeTicks += newTicksToday;
	}

	public static DateTime CurrentDateTime
	{
		get
		{
			return new DateTime
			{
				seconds = (int)(TicksToday / TicksPerIngameSecond),
				day = Day,
				year = Year,
				weekDay = WeekDayHelper.FromInt((int)(LifetimeDays % (ulong)WeekDayHelper.DaysOfWeek))
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
		result += (second.seconds - first.seconds) / secondsPerDay;
		return Mathf.Abs(result);
	}
	public static void IncrementDay ()
	{
		lifetimeTicks += (ulong)(TicksPerIngameSecond * secondsPerDay);
	}
}
