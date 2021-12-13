using System.Collections.Generic;

public enum WeekDay {
	Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday
}

public static class WeekDayHelper {
	public static int DaysInWeek => 7;

	private static List<WeekDay> OrderedDays => new()
	{
			WeekDay.Monday,
			WeekDay.Tuesday,
			WeekDay.Wednesday,
			WeekDay.Thursday,
			WeekDay.Friday,
			WeekDay.Saturday,
			WeekDay.Sunday
	};

	public static WeekDay GetFollowingDay (this WeekDay currentDay) {
		int index = OrderedDays.IndexOf (currentDay) + 1;
		if (index >= OrderedDays.Count)
			index = 0;
		return OrderedDays [index];
	}
	public static WeekDay FromInt (int day)
	{
		return OrderedDays[day];
	}
	public static int ToInt (this WeekDay day)
	{
		return OrderedDays.IndexOf(day);
	}
}
