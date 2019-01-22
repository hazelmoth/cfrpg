using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeekDay {
	Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday
}

public static class WeekDayMethods {
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
		List<WeekDay> orderedDays = new List<WeekDay>{
			WeekDay.Monday,
			WeekDay.Tuesday,
			WeekDay.Wednesday,
			WeekDay.Thursday,
			WeekDay.Friday,
			WeekDay.Saturday,
			WeekDay.Sunday
		};
		int index = orderedDays.IndexOf (currentDay) + 1;
		if (index >= orderedDays.Count)
			index = 0;
		return orderedDays [index];
	}
}