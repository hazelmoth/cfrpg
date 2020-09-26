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
			int hour = (int)(((float)seconds / secondsPerDay) * 24);

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

	public const int secondsPerDay = 86400; // Number of ingame seconds in an ingame day

	private const int ticksPerRealSecond = 60; // The number of ticks in a real second, at normal Time.timescale


	public static float timeScale = 48f; // Number of in-game seconds for every real second

	private const int clockStartYear = 2100; // The year that ticks count up from

	private static ulong lifetimeTicks; // Number of ticks since the clock start year at 12:00:00 am

	private static uint lastTickCount; // The number of ticks since game launch, as of the previous frame.

	private static int tickJump; // The number of ticks to be added to DeltaTicks on the current frame as 
								 // the result of a time jump. Resets every frame.

	public static int DeltaTicks { get; private set; } // How many ticks occurred in the previous frame
	public static ulong CurrentTick => lifetimeTicks;
	public static float TicksPerIngameSecond => ticksPerRealSecond / timeScale;
	public static float TimeAsFraction => TicksToday / (TicksPerIngameSecond * secondsPerDay);
	public static WeekDay DayOfWeek => WeekDayHelper.FromInt((int)(LifetimeDays % (ulong)WeekDayHelper.DaysOfWeek));


	private static double LifetimeSeconds => (double)lifetimeTicks / TicksPerIngameSecond;
	private static double LifetimeDays => LifetimeSeconds / secondsPerDay;
	private static int LifetimeYears => (int)(LifetimeDays / (ulong)Calendar.DaysInYear);
	private static uint TicksToday => (uint)(lifetimeTicks % (TicksPerIngameSecond * secondsPerDay)); // How many ticks have elapsed on the current day
	private static int Year => LifetimeYears + clockStartYear;
	private static int Day => (int)(LifetimeDays % (uint)Calendar.DaysInYear);
	private static int Second => (int)(LifetimeSeconds % 60);
	private static int Min => (int)(LifetimeSeconds % 3600) / 60;
	private static int Hour => (int)(LifetimeSeconds % secondsPerDay) / 3600;


	private void OnDestroy()
	{
		OnMinuteChanged = null;
	}

	private void Start() 
	{
		SetTime(0.5f);
		lastTickCount = (uint)Mathf.FloorToInt(Time.time * ticksPerRealSecond);
	}

	private void Update() 
	{
		int oldMin = Min;
		uint tickCount = (uint)Mathf.FloorToInt(Time.time * ticksPerRealSecond);
		DeltaTicks = (int)(tickCount - lastTickCount);
		DeltaTicks += tickJump;
		tickJump = 0;

		lifetimeTicks += (uint)DeltaTicks;
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
		tickJump += (int)(newTicksToday - TicksToday);
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
	public static float daysBetween(ulong first, ulong second)
	{
		ulong elapsedTicks;
		if (second > first) elapsedTicks = second - first;
		else elapsedTicks = first - second;

		return elapsedTicks / (TicksPerIngameSecond * secondsPerDay);
	}
	public static float daysBetween (DateTime first, DateTime second)
	{
		float result = 0;
		result += (second.year - first.year) * Calendar.DaysInYear;
		result += (second.day - first.day);
		result += (second.seconds - first.seconds) / secondsPerDay;
		return Mathf.Abs(result);
	}

	// Instantaneously advances time by 24 hours.
	public static void AdvanceDay()
	{
		tickJump += (int)(TicksPerIngameSecond * secondsPerDay);
	}

	// Instantaneously advances time by the given number of ticks.
	public static void TimeJump (int ticks)
	{
		tickJump += ticks;
	}
}
