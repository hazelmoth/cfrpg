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
			int hour = (int)(((float)seconds / SecondsPerDay) * 24);

			bool isPm = (hour >= 12);
			hour %= 12;
			if (hour == 0)
				hour = 12;
			string timeString;
			if (isPm)
				timeString = (hour + ":" + min.ToString("00") + " pm");
			else
				timeString = (hour + ":" + min.ToString("00") + " am");

			return $"{weekDay}, {Calendar.GetSeason(day).Name} {Calendar.GetDayOfSeason(day)} {year}, {timeString}";
		}
	}

	public delegate void TimeEvent();
	public static event TimeEvent OnMinuteChanged;

	/// The number of in-game seconds in an in-game day.
	public const int SecondsPerDay = 86400;

	/// The number of ticks in a real second, at normal Time.timescale.
	/// The real duration of a tick is defined by this value.
	public const int TicksPerRealSecond = 60; 

	/// The number of in-game seconds for every real second.
	/// This defines the length of an in-game second.
	public static float timeScale = 48f;

	private const int ClockStartYear = 1; // The year that ticks count up from

	private static uint lastTickCount; // The number of ticks since game launch, as of the previous frame.

	private static int tickJump; // The number of ticks to be added to DeltaTicks on the current frame as 
								 // the result of a time jump. Resets every frame.

	/// How many ticks occurred in the previous frame.
	public static int DeltaTicks { get; private set; }

	/// Number of ticks since the clock start year at 12:00:00 am.
	public static ulong CurrentTick { get; private set; }

	/// How many ticks occur during each second on the in-game clock.
	/// This value is derived from timescale and ticks per real second.
	public static float TicksPerInGameSecond => TicksPerRealSecond / timeScale;

	/// How many ticks occur during each day on the in-game clock.
	/// This value is derived from timescale and ticks per real second.
	public static float TicksPerInGameDay => TicksPerInGameSecond * SecondsPerDay;

	/// The current time of day, as a value between 0 and 1 (where 0 and 1 are midnight).
	public static float TimeOfDay => TicksToday / (TicksPerInGameSecond * SecondsPerDay);

	/// Which day of the week is today.
	public static WeekDay DayOfWeek => WeekDayHelper.FromInt((int)(LifetimeDays % (ulong)WeekDayHelper.DaysOfWeek));


	private static double LifetimeSeconds => (double)CurrentTick / TicksPerInGameSecond;
	private static double LifetimeDays => LifetimeSeconds / SecondsPerDay;
	private static int LifetimeYears => (int)(LifetimeDays / (ulong)Calendar.DaysInYear);
	private static uint TicksToday => (uint)(CurrentTick % (TicksPerInGameSecond * SecondsPerDay));
	private static int Year => LifetimeYears + ClockStartYear;
	private static int DayOfYear => (int)(LifetimeDays % (uint)Calendar.DaysInYear);
	private static int HourOfDay => (int)(LifetimeSeconds % SecondsPerDay) / 3600;
	private static int MinOfHour => (int)(LifetimeSeconds % 3600) / 60;
	private static int SecondOfMin => (int)(LifetimeSeconds % 60);


	private void OnDestroy()
	{
		OnMinuteChanged = null;
	}

	private void Start() 
	{
		lastTickCount = (uint)Mathf.FloorToInt(Time.time * TicksPerRealSecond);
	}

	private void Update() 
	{
		int oldMin = MinOfHour;
		uint tickCount = (uint)Mathf.FloorToInt(Time.time * TicksPerRealSecond);
		DeltaTicks = (int)(tickCount - lastTickCount);
		DeltaTicks += tickJump;
		tickJump = 0;

		CurrentTick += (uint)DeltaTicks;
		lastTickCount = tickCount;
		if (MinOfHour != oldMin)
		{
			OnMinuteChanged?.Invoke();
		}
	}

	/// Sets time to the given tick, without affecting DeltaTicks.
	public static void SetCurrentTick(ulong tick)
	{
		CurrentTick = tick;
	}

	/// Advances time to the given time of day, expressed as a float between 0 and 1 (where 0 and 1 are midnight).
	/// Advances to the next day if the given time is earlier than the current time.
	public static void SetTimeOfDay(float timeAsFraction)
	{
		timeAsFraction = Mathf.Clamp01(timeAsFraction);

		ulong newTicksToday = (ulong)(timeAsFraction * SecondsPerDay * TicksPerInGameSecond);
		int timeChange = (int)(newTicksToday - TicksToday);
		if (timeChange < 0)
		{
			// The target time is earlier than the current time!
			// Go to the next day instead.
			timeChange += (int)(TicksPerInGameSecond * SecondsPerDay);
		}
		TimeJump(timeChange);
	}

	public static DateTime CurrentDateTime
	{
		get
		{
			return new DateTime
			{
				seconds = (int)(TicksToday / TicksPerInGameSecond),
				day = DayOfYear,
				year = Year,
				weekDay = WeekDayHelper.FromInt((int)(LifetimeDays % (ulong)WeekDayHelper.DaysOfWeek))
			};
		}
	}

	/// The time of day, formatted e.g. "4:22 pm".
	public static string FormattedTime
	{
		get
		{
			int min = MinOfHour;
			int hour = HourOfDay;
			bool isPm = (hour >= 12);
			hour %= 12;
			if (hour == 0) hour = 12;
			if (isPm)
				return (hour + ":" + min.ToString("00") + " pm");
			else
				return (hour + ":" + min.ToString("00") + " am");
		}
	}

	/// The precise number of in-game days between the specified ticks.
	public static float DaysBetween(ulong first, ulong second)
	{
		ulong elapsedTicks;
		if (second > first) elapsedTicks = second - first;
		else elapsedTicks = first - second;

		return elapsedTicks / (TicksPerInGameSecond * SecondsPerDay);
	}

	/// Instantaneously advances time by the given number of ticks.
	private static void TimeJump (int ticks)
	{
		if (ticks < 0)
		{
			Debug.LogError("Timejumping by a negative number of ticks! Travelling backwards in time is not supported.");
		}
		tickJump += ticks;
	}
}
