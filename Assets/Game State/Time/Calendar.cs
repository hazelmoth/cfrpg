using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Calendar
{
	public class Month
	{
		public Month(string name, int numDays)
		{
			this.Name = name;
			this.NumDays = numDays;
		}

		public string Name { get; }
		public int NumDays { get; }
	}

	public static List<Month> Months { get; } = new List<Month>
	{
		new Month("Janumonth", 10),
		new Month("Midmonth", 12),
		new Month("Decembromonth", 11)
	};

	public static int DaysInYear {
		get
		{
			int result = 0;
			foreach (Month m in Months)
			{
				result += m.NumDays;
			}
			return result;
		} 
	}

	public static int GetDayOfMonth (int dayOfYear)
	{
		int remaining = dayOfYear;
		while (true)
		{
			foreach (Month m in Months)
			{
				if (remaining > m.NumDays)
				{
					remaining -= m.NumDays;
				}
				else
				{
					return remaining;
				}
			}
		}
	}

	// not zero-indexed
	public static Month GetMonth(int dayOfYear)
	{
		int remaining = dayOfYear;
		while (true)
		{
			foreach (Month m in Months)
			{
				if (remaining > m.NumDays)
				{
					remaining -= m.NumDays;
				}
				else
				{
					return m;
				}
			}
		}
	}
	public static Month GetFollowingMonth (Month month)
	{
		int index = -1;
		for (int i = 0; i < Months.Count; i++)
		{
			if (Months[i].Equals(month))
				index = i;
		}
		index++;
		index = index % Months.Count;
		return Months[index];
	}
}
