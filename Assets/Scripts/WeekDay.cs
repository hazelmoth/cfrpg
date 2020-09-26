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

	public static WeekDay WeekdayFromString (string day) {
		switch (day.ToLower()) {
		case "monday":
			return WeekDay.Monday;
		case "tuesday":
			return WeekDay.Tuesday;
		case "wednesday":
			return WeekDay.Wednesday;
		case "thursday":
			return WeekDay.Thursday;
		case "friday":
			return WeekDay.Friday;
		case "saturday":
			return WeekDay.Saturday;
		default:
			return WeekDay.Sunday;
		}
	}
	public static string ToString (this WeekDay day) {
		switch (day) {
		case WeekDay.Monday:
			return "Monday";
		case WeekDay.Tuesday:
			return "Tuesday";
		case WeekDay.Wednesday:
			return "Wednesday";
		case WeekDay.Thursday:
			return "Thursday";
		case WeekDay.Friday:
			return "Friday";
		case WeekDay.Saturday:
			return "Saturday";
		default:
			return "Sunday";
		}
	}
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