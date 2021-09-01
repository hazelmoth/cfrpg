using System.Collections.Generic;

public enum WeekDay {
	Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday
}

public static class WeekDayHelper {
	public static int DaysOfWeek => 7;

	private static List<WeekDay> OrderedDays => new List<WeekDay>{
			WeekDay.Monday,
			WeekDay.Tuesday,
			WeekDay.Wednesday,
			WeekDay.Thursday,
			WeekDay.Friday,
			WeekDay.Saturday,
			WeekDay.Sunday
		};

	public static WeekDay GetNextDay (WeekDay currentDay) {
		int index = OrderedDays.IndexOf (currentDay) + 1;
		if (index >= OrderedDays.Count)
			index = 0;
		return OrderedDays [index];
	}
	public static WeekDay FromInt (int day)
	{
		return OrderedDays[day];
	}
}
